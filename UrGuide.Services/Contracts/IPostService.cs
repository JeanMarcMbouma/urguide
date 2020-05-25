using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IPostService
    {
        Task<Result<PostModel>> CreatePostAsync(PostCreationModel model, CancellationToken cancellationToken);
        Task<Result<IEnumerable<PostModel>>> GetLast10PostsAsync(CancellationToken cancellationToken);
        Task<Result<IEnumerable<PostModel>>> GetLast100PostsAsync(CancellationToken cancellationToken);
        Task<Result<bool>> UpdatePostAsync(PostUpdateModel model, CancellationToken cancellationToken);
        Task<Result<bool>> DeletePostAsync(string id, CancellationToken cancellationToken);
        Task<Result<bool>> UpdatePostAttributesAsync(string id, SetAttribute[] attributes, CancellationToken cancellationToken);
        Task<Result<IEnumerable<ItineraryModel>>> GetItinerariesAsync(string postId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<PostModel>>> GetTop10PostsAsync(CancellationToken cancellationToken);
        Task<Result<IEnumerable<PostModel>>> GetTop100PostsAsync(CancellationToken cancellationToken);
        Task<Result<bool>> ReserveSeatsAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken);
        Task<Result<bool>> UpdateSeatReservationAsync(SeatReservationModel seatReservation, CancellationToken cancellationToken);
        Task<Result<bool>> CancelReservationAsync(string postId, CancellationToken cancellationToken);
        Task<Result<bool>> RecordUserReactionAsync(UserReactionModel userReaction, CancellationToken cancellationToken);
        Task<Result<PostModel>> GetByIdAsync(string postId, CancellationToken cancellationToken);
        Task<Result<PagedList<PostModel>>> GetOwnPostsAsync(PostPagination pagination, CancellationToken cancellationToken);
    }
}
