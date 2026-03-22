using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Messages;
using UrGuide.Model.Messages;

namespace UrGuide.WebApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> _onlineUsers = new();
        private static readonly object _lock = new();

        private readonly UrGuideContext _context;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(UrGuideContext context, ILogger<ChatHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetUserId() =>
            Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        private string GetUserName() =>
            Context.User?.FindFirstValue(ClaimTypes.Name)
            ?? Context.User?.FindFirstValue("name")
            ?? "User";

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                await base.OnConnectedAsync();
                return;
            }

            bool isNewOnline;
            lock (_lock)
            {
                if (!_onlineUsers.TryGetValue(userId, out var connections))
                {
                    connections = new HashSet<string>();
                    _onlineUsers[userId] = connections;
                }
                isNewOnline = connections.Count == 0;
                connections.Add(Context.ConnectionId);
            }

            if (isNewOnline)
            {
                await Clients.Others.UserOnline(userId);
            }

            // Auto-join all conversation groups the user belongs to
            var conversationIds = await _context.Conversations
                .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var conversationId in conversationIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                bool isNowOffline;
                lock (_lock)
                {
                    if (_onlineUsers.TryGetValue(userId, out var connections))
                    {
                        connections.Remove(Context.ConnectionId);
                        isNowOffline = connections.Count == 0;
                        if (isNowOffline)
                        {
                            _onlineUsers.TryRemove(userId, out _);
                        }
                    }
                    else
                    {
                        isNowOffline = false;
                    }
                }

                if (isNowOffline)
                {
                    await Clients.Others.UserOffline(userId);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Send a message to a conversation, persist to DB, and broadcast to participants.
        /// </summary>
        public async Task SendMessage(string conversationId, string content)
        {
            var userId = GetUserId();
            var userName = GetUserName();
            if (string.IsNullOrEmpty(userId)) return;

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId &&
                    (c.Participant1Id == userId || c.Participant2Id == userId));

            if (conversation == null) return;

            var entity = new MessageEntity
            {
                Id = Guid.NewGuid().ToString(),
                ConversationId = conversationId,
                SenderId = userId,
                SenderName = userName,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false,
            };

            _context.MessageEntities.Add(entity);

            conversation.LastMessage = content;
            conversation.LastMessageAt = entity.SentAt;

            await _context.SaveChangesAsync();

            var dto = new ChatMessageDto
            {
                Id = entity.Id,
                ConversationId = entity.ConversationId,
                SenderId = entity.SenderId,
                SenderName = entity.SenderName,
                Content = entity.Content,
                SentAt = entity.SentAt,
                IsRead = entity.IsRead,
            };

            await Clients.Group(conversationId).ReceiveMessage(dto);
        }

        /// <summary>
        /// Notify participants that a user is typing.
        /// </summary>
        public async Task SendTypingIndicator(string conversationId)
        {
            var userId = GetUserId();
            var userName = GetUserName();
            if (string.IsNullOrEmpty(userId)) return;

            await Clients.OthersInGroup(conversationId).UserTyping(conversationId, userId, userName);
        }

        /// <summary>
        /// Notify participants that a user stopped typing.
        /// </summary>
        public async Task SendStoppedTyping(string conversationId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return;

            await Clients.OthersInGroup(conversationId).UserStoppedTyping(conversationId, userId);
        }

        /// <summary>
        /// Mark a message as read and notify the sender.
        /// </summary>
        public async Task MarkMessageAsRead(string conversationId, string messageId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var message = await _context.MessageEntities
                .FirstOrDefaultAsync(m => m.Id == messageId
                    && m.ConversationId == conversationId
                    && m.SenderId != userId);

            if (message == null || message.IsRead) return;

            message.IsRead = true;
            await _context.SaveChangesAsync();

            await Clients.Group(conversationId).MessageRead(conversationId, messageId, userId);
        }

        /// <summary>
        /// Share file metadata with a conversation. The actual file upload is handled separately via API.
        /// </summary>
        public async Task ShareFile(string conversationId, string messageId, string fileName, string fileUrl, long fileSize, string contentType)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId &&
                    (c.Participant1Id == userId || c.Participant2Id == userId));

            if (conversation == null) return;

            var messageExists = await _context.MessageEntities
                .AnyAsync(m => m.Id == messageId && m.ConversationId == conversationId);

            if (!messageExists) return;

            var attachment = new FileAttachment
            {
                Id = Guid.NewGuid().ToString(),
                MessageId = messageId,
                FileName = fileName,
                FileUrl = fileUrl,
                FileSize = fileSize,
                ContentType = contentType,
                UploadedAt = DateTime.UtcNow,
            };

            _context.FileAttachments.Add(attachment);
            await _context.SaveChangesAsync();

            var dto = new FileAttachmentDto
            {
                Id = attachment.Id,
                MessageId = attachment.MessageId,
                FileName = attachment.FileName,
                FileUrl = attachment.FileUrl,
                FileSize = attachment.FileSize,
                ContentType = attachment.ContentType,
                UploadedAt = attachment.UploadedAt,
            };

            await Clients.Group(conversationId).FileShared(conversationId, dto);
        }

        /// <summary>
        /// Join a conversation group to receive real-time messages.
        /// </summary>
        public async Task JoinConversation(string conversationId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return;

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId &&
                    (c.Participant1Id == userId || c.Participant2Id == userId));

            if (conversation == null) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        /// <summary>
        /// Leave a conversation group to stop receiving real-time messages.
        /// </summary>
        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        /// <summary>
        /// Get a list of currently online user IDs.
        /// </summary>
        public Task<List<string>> GetOnlineUsers()
        {
            List<string> onlineUserIds;
            lock (_lock)
            {
                onlineUserIds = _onlineUsers.Keys.ToList();
            }
            return Task.FromResult(onlineUserIds);
        }
    }
}
