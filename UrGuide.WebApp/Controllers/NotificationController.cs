using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Messages;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    public class NotificationController : Controller
    {
        public NotificationController(IUserNotificationService notificationService)
        {
            NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public IUserNotificationService NotificationService { get; }

        [HttpGet("unread")]
        [ProducesDefaultResponseType(typeof(PagedList<Notification>))]
        public async Task<IActionResult> GetUnread([FromQuery]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await NotificationService.GetUnreadAsync(pagination, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(Notification))]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            var result = await NotificationService.GetNotificationAsync(id, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpGet("all")]
        [ProducesDefaultResponseType(typeof(PagedList<Notification>))]
        public async Task<IActionResult> GetAll([FromQuery]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await NotificationService.GetAllAsync(pagination, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        [HttpPost("chat")]
        [Authorize]
        public async Task<IActionResult> NewMessage([FromBody]ChatMessage message, [FromServices]IUserContext userContext)
        {
            await NotificationService.NotifyAsync(new CreateNotification
            {
                AuthorId = userContext.UserId,
                UserId = message.To,
                Content = message.Content,
                IsSystem = false,
                ReferenceLink = message.ReferenceLink
            });
            return NoContent();
        }

        [HttpPut("{id}/mark_as_read")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> MarkAsRead(string id, CancellationToken cancellationToken)
        {
            var result = await NotificationService.MarkAsReadAsync(id, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }
    }
}
