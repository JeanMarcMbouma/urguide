using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{


    [ApiController]
    [Route("posts")]
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
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("{userId}/all")]
        [ProducesDefaultResponseType(typeof(PagedList<PostModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersPosts(string userId, SearchParameters pagination, CancellationToken cancellationToken)
        {
            var result = await _postService.GetPostsByUserId(userId, pagination, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("last10")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await _postService.GetLast10PostsAsync(cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("last100")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Last100(CancellationToken cancellationToken)
        {
            var result = await _postService.GetLast100PostsAsync(cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("top10")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Top10(CancellationToken cancellationToken)
        {
            var result = await _postService.GetTop10PostsAsync(cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("{postId}/retrieve")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(PostModel))]
        public async Task<IActionResult> GetOne(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.GetByIdAsync(postId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("top100")]
        [AllowAnonymous]
        [ProducesDefaultResponseType(typeof(IEnumerable<PostModel>))]
        public async Task<IActionResult> Top100(CancellationToken cancellationToken)
        {
            var result = await _postService.GetTop100PostsAsync(cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("create")]
        [ProducesDefaultResponseType(typeof(PostModel))]
        public async Task<IActionResult> Create([FromBody]PostCreationModel model, CancellationToken cancellationToken)
        {
            var result = await _postService.CreatePostAsync(model, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }


        [HttpPut("{postId}/update")]
        public async Task<IActionResult> Edit(string postId, PostUpdateModel model, CancellationToken cancellationToken)
        {
            if (postId != model.Id)
            {
                return BadRequest();
            }

            var result = await _postService.UpdatePostAsync(model, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPut("{postId}/attributes/{name}/update")]
        public async Task<IActionResult> EditAttribute(string postId, string name, SetAttribute attribute, CancellationToken cancellationToken)
        {
            if (name != attribute.Name)
            {
                return BadRequest();
            }

            var result = await _postService.UpdatePostAttributesAsync(postId, new[] { attribute }, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPut("{postId}/attributes/batchupdate")]
        public async Task<IActionResult> EditAttributes(string postId, SetAttribute[] attributes, CancellationToken cancellationToken)
        {
            if (attributes == null)
                return BadRequest();
            var result = await _postService.UpdatePostAttributesAsync(postId, attributes, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpDelete("{postId}/attributes/{name}/remove")]
        public async Task<IActionResult> RemoveAttribute(string postId, string name, CancellationToken cancellationToken)
        {
            
            var result = await _postService.UpdatePostAttributesAsync(postId, new[] { new SetAttribute { Name = name, Value = string.Empty } }, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpDelete("{postId}/remove")]
        public async Task<IActionResult> DeletePost(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.DeletePostAsync(postId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpGet("{postId}/itineraries")]
        [ProducesDefaultResponseType(typeof(IEnumerable<ItineraryModel>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetItineraries(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.GetItinerariesAsync(postId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("{postId}/makereservation")]
        public async Task<IActionResult> Reserve(string postId, SeatReservationModel seatReservation, CancellationToken cancellationToken)
        {
            if (seatReservation.PostId != postId)
                return BadRequest();
            var result = await _postService.ReserveSeatsAsync(seatReservation, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPut("{postId}/editreservation")]
        public async Task<IActionResult> EditReservation(string postId, SeatReservationModel seatReservation, CancellationToken cancellationToken)
        {
            if (seatReservation.PostId != postId)
                return BadRequest();
            var result = await _postService.ReserveSeatsAsync(seatReservation, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("{postId}/cancelreservation")]
        public async Task<IActionResult> CancelReservation(string postId, CancellationToken cancellationToken)
        {
            var result = await _postService.CancelReservationAsync(postId, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }

        [HttpPost("{postId}/reaction")]
        public async Task<IActionResult> RecordUserReaction(string postId, UserReactionModel userReaction, CancellationToken cancellationToken)
        {
            if (userReaction.PostId != postId)
                return BadRequest();
            var result = await _postService.RecordUserReactionAsync(userReaction, cancellationToken);
            return result.HasError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Data);
        }
    }
}
