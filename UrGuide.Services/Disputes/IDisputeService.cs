using System.Threading.Tasks;
using UrGuide.Model.Disputes;

namespace UrGuide.Services.Disputes
{
    public interface IDisputeService
    {
        Task<DisputeDto> CreateDisputeAsync(string userId, CreateDisputeRequest request);
        Task<DisputeDto> GetDisputeAsync(string disputeId);
        Task<DisputeListResponse> GetUserDisputesAsync(string userId, int page = 1, int pageSize = 20);
        Task<DisputeListResponse> GetAdminDisputeQueueAsync(int page = 1, int pageSize = 20, int? status = null, int? priority = null);
        Task<DisputeEvidenceDto> SubmitEvidenceAsync(string userId, string disputeId, SubmitEvidenceRequest request);
        Task<DisputeMessageDto> AddMessageAsync(string userId, string disputeId, DisputeMessageRequest request);
        Task<bool> AssignDisputeAsync(string adminId, string disputeId);
        Task<DisputeDto> ResolveDisputeAsync(string adminId, string disputeId, ResolveDisputeRequest request);
        Task<bool> EscalateDisputeAsync(string adminId, string disputeId);
        Task<DisputeStatsDto> GetDisputeStatsAsync();
    }
}
