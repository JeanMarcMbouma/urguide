using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        // In-memory store keyed by conversationId
        private static readonly ConcurrentDictionary<string, ConversationSummary> _conversations = new();
        private static readonly ConcurrentDictionary<string, List<MessageItem>> _messages = new();

        public MessagesController(ILogger<MessagesController> logger)
        {
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        private string GetUserName() => User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name") ?? "Guide";

        /// <summary>
        /// Get all conversations for the authenticated user
        /// </summary>
        [HttpGet("conversations")]
        public IActionResult GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var userConvos = _conversations.Values
                .Where(c => c.ParticipantId == userId || c.Id.StartsWith(userId) || c.Id.EndsWith(userId))
                .OrderByDescending(c => c.LastMessageAt)
                .ToList();

            var paged = userConvos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new ConversationListResponse
            {
                Conversations = paged,
                TotalCount = userConvos.Count,
                Page = page,
                PageSize = pageSize,
            });
        }

        /// <summary>
        /// Get messages for a specific conversation
        /// </summary>
        [HttpGet("conversations/{conversationId}")]
        public IActionResult GetMessages(string conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var msgs = _messages.TryGetValue(conversationId, out var list)
                ? list.OrderBy(m => m.SentAt).ToList()
                : new List<MessageItem>();

            var paged = msgs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new MessageListResponse
            {
                Messages = paged,
                TotalCount = msgs.Count,
                Page = page,
                PageSize = pageSize,
            });
        }

        /// <summary>
        /// Send a message
        /// </summary>
        [HttpPost]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var msg = new MessageItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ConversationId = request.ConversationId,
                    SenderId = userId,
                    SenderName = GetUserName(),
                    Content = request.Content,
                    SentAt = DateTime.UtcNow,
                    IsRead = false,
                };

                if (!_messages.ContainsKey(request.ConversationId))
                    _messages.TryAdd(request.ConversationId, new List<MessageItem>());

                lock (_messages[request.ConversationId])
                    _messages[request.ConversationId].Add(msg);

                // Update conversation summary
                if (_conversations.TryGetValue(request.ConversationId, out var convo))
                {
                    convo.LastMessage = request.Content;
                    convo.LastMessageAt = msg.SentAt;
                }

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
        public IActionResult MarkAsRead(string conversationId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (_messages.TryGetValue(conversationId, out var msgs))
            {
                foreach (var msg in msgs.Where(m => m.SenderId != userId))
                    msg.IsRead = true;
            }

            if (_conversations.TryGetValue(conversationId, out var convoSummary))
                convoSummary.UnreadCount = 0;

            return Ok(new { message = "Conversation marked as read." });
        }
    }
}
