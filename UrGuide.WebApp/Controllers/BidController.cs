using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;
using BbQ.Outcome;

namespace UrGuide.WebApp.Controllers
{
    [Route("api/bid")]
    [ApiController]
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class BidController : ControllerBase
    {
        public BidController(IBidService bidService)
        {
            BidService = bidService ?? throw new ArgumentNullException(nameof(bidService));
        }

        public IBidService BidService { get; }

        [HttpPost("{postId}/newbid")]
        [ProducesResponseType(200, Type = typeof(PostModel))]
        public async Task<IActionResult> NewBid(string postId, BidModel model, CancellationToken cancellationToken)
        {
            if(model.PostId != postId)
            {
                return BadRequest(ErrorEnvelop.CreateFromOutcome(Result.Of<PostModel>().WithErrors("Invalid object").Errors));
            }
            var result = await BidService.OpenBidAsync(model, cancellationToken);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("{postId}/accept")]
        [ProducesResponseType(200, Type = typeof(PostModel))]
        public async Task<IActionResult> Accept(string postId, CancellationToken cancellationToken)
        {
            var result = await BidService.AcceptBidAsync(postId, cancellationToken);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("{postId}/reject")]
        [ProducesResponseType(200, Type= typeof(PostModel))]
        public async Task<IActionResult> Reject(string postId, CancellationToken cancellationToken)
        {
            var result = await BidService.RejectBidAsync(postId, cancellationToken);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("{postId}/history")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BidHistoryModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetHistory(string postId, CancellationToken cancellationToken)
        {
            var result = await BidService.GetBidHistoryAsync(postId, cancellationToken);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }
    }
}