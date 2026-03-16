using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Shared;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class FeedbackController : Controller
    {
        public FeedbackController(IFeedbackService feedbackService)
        {
            FeedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }

        public IFeedbackService FeedbackService { get; }

        [HttpPost("posts/{postId}/feedback")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> PostFeedback(string postId, [FromBody]FeedbackModel feedback, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.AddPostFeedbackAsync(postId, feedback, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("users/{userId}/feedback")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UserFeedback(string userId, [FromBody]FeedbackModel feedback, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.AddUserFeedbackAsync(userId, feedback, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("feedback/users/{userId}")]
        [ProducesDefaultResponseType(typeof(PagedList<AuthoredFeedback>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserFeedback(string userId, [FromQuery][Bind(nameof(PaginationParameters.PageNumber))]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.GetUserFeedback(userId, pagination, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }


        [HttpGet("feedback/posts/{postId}")]
        [ProducesDefaultResponseType(typeof(PagedList<AuthoredFeedback>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostFeedback(string postId, [FromQuery][Bind(nameof(PaginationParameters.PageNumber))]PaginationParameters pagination, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.GetPostFeedback(postId, pagination, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("feedback/{feedbackId}/respond")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> RespondToFeedback(string feedbackId, [FromBody] FeedbackResponseModel response, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.RespondToFeedbackAsync(feedbackId, response.Response, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }
    }
}
