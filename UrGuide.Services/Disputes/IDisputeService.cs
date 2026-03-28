using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Disputes;

namespace UrGuide.Services.Disputes
{
    public interface IDisputeService
    {
        Task<DisputeDto> CreateDisputeAsync(string userId, CreateDisputeRequest request, CancellationToken cancellationToken = default);
        Task<DisputeDto> GetDisputeAsync(string disputeId, CancellationToken cancellationToken = default);
        Task<DisputeListResponse> GetUserDisputesAsync(string userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<DisputeListResponse> GetAdminDisputeQueueAsync(int page = 1, int pageSize = 20, int? status = null, int? priority = null, CancellationToken cancellationToken = default);
        Task<DisputeEvidenceDto> SubmitEvidenceAsync(string userId, string disputeId, SubmitEvidenceRequest request, CancellationToken cancellationToken = default);
        Task<DisputeMessageDto> AddMessageAsync(string userId, string disputeId, DisputeMessageRequest request, CancellationToken cancellationToken = default);
        Task<bool> AssignDisputeAsync(string adminId, string disputeId, CancellationToken cancellationToken = default);
        Task<DisputeDto> ResolveDisputeAsync(string adminId, string disputeId, ResolveDisputeRequest request, CancellationToken cancellationToken = default);
        Task<bool> EscalateDisputeAsync(string adminId, string disputeId, CancellationToken cancellationToken = default);
        Task<DisputeStatsDto> GetDisputeStatsAsync(CancellationToken cancellationToken = default);
    }
}
