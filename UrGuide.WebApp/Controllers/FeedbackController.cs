using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Shared;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        public FeedbackController(IFeedbackService feedbackService)
        {
            FeedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        }

        public IFeedbackService FeedbackService { get; }

        [HttpPost("posts/{postId}/feedback")]
        public async Task<IActionResult> PostFeedback(string postId, [FromBody]FeedbackModel feedback, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.AddPostFeedbackAsync(postId, feedback, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("users/{userId}/feedback")]
        public async Task<IActionResult> UserFeedback(string postId, [FromBody]FeedbackModel feedback, CancellationToken cancellationToken)
        {
            var result = await FeedbackService.AddUserFeedbackAsync(postId, feedback, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }
    }
}
