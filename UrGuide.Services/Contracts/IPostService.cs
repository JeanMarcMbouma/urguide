using System.Collections.Generic;
using System.Threading;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IPostService
    {
        Task<Outcome<PostModel>> CreatePostAsync(PostCreationModel model, CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<PostModel>>> GetLast10PostsAsync(CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<PostModel>>> GetLast100PostsAsync(CancellationToken cancellationToken);
        Task<Outcome<bool>> UpdatePostAsync(PostUpdateModel model, CancellationToken cancellationToken);
        Task<Outcome<bool>> DeletePostAsync(string id, CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<ItineraryModel>>> GetItinerariesAsync(string postId, CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<PostModel>>> GetTop10PostsAsync(CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<PostModel>>> GetTop100PostsAsync(CancellationToken cancellationToken);
        Task<Outcome<bool>> ReserveSeatsAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken);
        Task<Outcome<bool>> UpdateSeatReservationAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken);
        Task<Outcome<bool>> CancelReservationAsync(string postId, CancellationToken cancellationToken);
        Task<Outcome<bool>> RecordUserReactionAsync(UserReactionModel userReaction, CancellationToken cancellationToken);
        Task<Outcome<PagedList<PostModel>>> GetPostsByUserId(string userId, SearchParameters pagination, CancellationToken cancellationToken);
        Task<Outcome<PostModel>> GetByIdAsync(string postId, CancellationToken cancellationToken);
        Task<Outcome<PagedList<PostModel>>> GetOwnPostsAsync(SearchParameters pagination, CancellationToken cancellationToken);
        Task<Outcome<PagedList<PostModel>>> GetPostsAsync(SearchParameters pagination, CancellationToken cancellationToken);
    }
}
