using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Messages;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly UrGuideContext _context;

        public MessagesController(ILogger<MessagesController> logger, UrGuideContext context)
        {
            _logger = logger;
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name") ?? "Guide";

        /// <summary>
        /// Get all conversations for the authenticated user
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var query = _context.Conversations
                .Where(c => c.Participant1Id == userId || c.Participant2Id == userId)
                .OrderByDescending(c => c.LastMessageAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var conversations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            // Count unread messages per conversation for this user
            var convoIds = conversations.Select(c => c.Id).ToList();
            var unreadCounts = await _context.MessageEntities
                .Where(m => convoIds.Contains(m.ConversationId) && m.SenderId != userId && !m.IsRead)
                .GroupBy(m => m.ConversationId)
                .Select(g => new { ConversationId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.ConversationId, g => g.Count, cancellationToken);

            var summaries = conversations.Select(c =>
            {
                // Determine the "other" participant for display
                var isParticipant1 = c.Participant1Id == userId;
                return new ConversationSummary
                {
                    Id = c.Id,
                    ParticipantId = isParticipant1 ? c.Participant2Id : c.Participant1Id,
                    ParticipantName = isParticipant1 ? c.Participant2Name : c.Participant1Name,
                    LastMessage = c.LastMessage,
                    LastMessageAt = c.LastMessageAt,
                    UnreadCount = unreadCounts.GetValueOrDefault(c.Id, 0),
                };
            }).ToList();

            return Ok(new ConversationListResponse
            {
                Conversations = summaries,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            });
        }

        /// <summary>
        /// Get messages for a specific conversation
        /// </summary>
        [HttpGet("conversations/{conversationId}")]
        public async Task<IActionResult> GetMessages(string conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Verify the user is a participant
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId &&
                    (c.Participant1Id == userId || c.Participant2Id == userId), cancellationToken);

            if (conversation == null)
                return NotFound(new { error = "Conversation not found." });

            var query = _context.MessageEntities
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var messages = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MessageItem
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    SenderName = m.SenderName,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                })
                .ToListAsync(cancellationToken);

            return Ok(new MessageListResponse
            {
                Messages = messages,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            });
        }

        /// <summary>
        /// Send a message
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(c => c.Id == request.ConversationId &&
                        (c.Participant1Id == userId || c.Participant2Id == userId), cancellationToken);

                if (conversation == null)
                    return NotFound(new { error = "Conversation not found." });

                var entity = new MessageEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = request.ConversationId,
                    SenderId = userId,
                    SenderName = GetUserName(),
                    Content = request.Content,
                    SentAt = DateTime.UtcNow,
                    IsRead = false,
                };

                _context.MessageEntities.Add(entity);

                // Update conversation summary
                conversation.LastMessage = request.Content;
                conversation.LastMessageAt = entity.SentAt;

                await _context.SaveChangesAsync(cancellationToken);

                var msg = new MessageItem
                {
                    Id = entity.Id,
                    ConversationId = entity.ConversationId,
                    SenderId = entity.SenderId,
                    SenderName = entity.SenderName,
                    Content = entity.Content,
                    SentAt = entity.SentAt,
                    IsRead = entity.IsRead,
                };

                return Ok(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return StatusCode(500, new { error = "An error occurred while sending the message" });
            }
        }

        /// <summary>
        /// Mark conversation as read
        /// </summary>
        [HttpPut("conversations/{conversationId}/read")]
        public async Task<IActionResult> MarkAsRead(string conversationId, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId &&
                    (c.Participant1Id == userId || c.Participant2Id == userId), cancellationToken);

            if (conversation == null)
                return NotFound(new { error = "Conversation not found." });

            var unreadMessages = await _context.MessageEntities
                .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
                .ToListAsync(cancellationToken);

            foreach (var msg in unreadMessages)
                msg.IsRead = true;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new { message = "Conversation marked as read." });
        }
    }
}
