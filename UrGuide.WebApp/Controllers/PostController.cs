using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{


    [ApiController]
    [Route("api/posts")]
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService ?? throw new ArgumentNullException(nameof(postService));
        }
        
        [HttpPost("owned")]
        [ProducesDefaultResponseType(typeof(PagedList<PostModel>))]
        public async Task<IActionResult> GetOwn(SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _postService.GetOwnPostsAsync(pagination, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("{userId}/all")]
        [ProducesDefaultResponseType(typeof(PagedList<PostModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersPosts(string userId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _postService.GetPostsByUserId(userId, pagination, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("search")]
        [ProducesDefaultResponseType(typeof(PagedList<PostModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> SearchPost([FromBody]SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _postService.GetPostsAsync(pagination, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("last10")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _postService.GetLast10PostsAsync(cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("last100")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Last100(CancellationToken cancellationToken)
        {
            var result = await _postService.GetLast100PostsAsync(cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("top10")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Top10(CancellationToken cancellationToken)
        {
            var result = await _postService.GetTop10PostsAsync(cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("{postId}/retrieve")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(PostModel))]
        public async Task<IActionResult> GetOne(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.GetByIdAsync(postId, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("top100")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Top100(CancellationToken cancellationToken)
        {
            var result = await _postService.GetTop100PostsAsync(cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("create")]
        [ProducesDefaultResponseType(typeof(PostModel))]
        public async Task<IActionResult> Create([FromBody]PostCreationModel model, CancellationToken cancellationToken)
        {
            var result = await _postService.CreatePostAsync(model, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }


        [HttpPut("{postId}/update")]
        public async Task<IActionResult> Edit(string postId, PostUpdateModel model, CancellationToken cancellationToken)
        {
            if (postId != model.Id)
            {
                return BadRequest();
            }

            var result = await _postService.UpdatePostAsync(model, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }


        [HttpDelete("{postId}/remove")]
        public async Task<IActionResult> DeletePost(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.DeletePostAsync(postId, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpGet("{postId}/itineraries")]
        [ProducesDefaultResponseType(typeof(IEnumerable<ItineraryModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetItineraries(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.GetItinerariesAsync(postId, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("{postId}/makereservation")]
        public async Task<IActionResult> Reserve(string postId, SeatReservationModel seatReservation, CancellationToken cancellationToken)
        {
            if (seatReservation.PostId != postId)
                return BadRequest();
            var result = await _postService.ReserveSeatsAsync(seatReservation, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPut("{postId}/editreservation")]
        public async Task<IActionResult> EditReservation(string postId, SeatReservationModel seatReservation, CancellationToken cancellationToken)
        {
            if (seatReservation.PostId != postId)
                return BadRequest();
            var result = await _postService.ReserveSeatsAsync(seatReservation, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("{postId}/cancelreservation")]
        public async Task<IActionResult> CancelReservation(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.CancelReservationAsync(postId, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [HttpPost("{postId}/reaction")]
        public async Task<IActionResult> RecordUserReaction(string postId, UserReactionModel userReaction, CancellationToken cancellationToken)
        {
            if (userReaction.PostId != postId)
                return BadRequest();
            var result = await _postService.RecordUserReactionAsync(userReaction, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : (IActionResult)Ok(result.Value);
        }
    }
}
