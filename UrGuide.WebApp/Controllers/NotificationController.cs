using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("notifications")]
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
            return result.HasError ? (IActionResult)BadRequest(ErrorEnvelop.Create(result.Errors)) : Ok(result.Data);
        }

        [HttpGet("all")]
        [ProducesDefaultResponseType(typeof(PagedList<Notification>))]
        public async Task<IActionResult> GetAll([FromQuery]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await NotificationService.GetAllAsync(pagination, cancellationToken);
            return result.HasError ? (IActionResult)BadRequest(ErrorEnvelop.Create(result.Errors)) : Ok(result.Data);
        }
    }
}
