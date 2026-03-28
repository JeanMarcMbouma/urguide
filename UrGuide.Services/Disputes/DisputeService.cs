using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Disputes;
using UrGuide.Model.Disputes;

namespace UrGuide.Services.Disputes
{
    public class DisputeService : IDisputeService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<DisputeService> _logger;

        public DisputeService(UrGuideContext context, ILogger<DisputeService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DisputeDto> CreateDisputeAsync(string userId, CreateDisputeRequest request, CancellationToken cancellationToken = default)
        {
            var booking = await _context.Set<Data.Entities.Tour.Booking>()
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.BookingId == request.BookingId, cancellationToken);

            if (booking == null)
            {
                throw new ArgumentException("Booking not found");
            }

            // Validate the caller is either the booking author or the tour guide
            if (booking.AuthorId != userId && booking.Tour?.AuthorId != userId)
            {
                throw new InvalidOperationException("You are not a participant of this booking");
            }

            // Determine the other party: if the filer is the booking author, the dispute is against the tour guide; otherwise against the booking author
            var againstUserId = booking.AuthorId == userId
                ? booking.Tour?.AuthorId ?? string.Empty
                : booking.AuthorId;

            var dispute = new Dispute
            {
                DisputeId = Guid.NewGuid().ToString(),
                BookingId = request.BookingId,
                FiledBy = userId,
                AgainstUserId = againstUserId,
                Title = request.Title,
                Description = request.Description,
                Category = (DisputeCategory)request.Category,
                Status = DisputeStatus.Open,
                Priority = DisputePriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Disputes.Add(dispute);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Dispute {DisputeId} created by user {UserId} for booking {BookingId}", dispute.DisputeId, userId, request.BookingId);

            return MapToDto(dispute);
        }

        public async Task<DisputeDto> GetDisputeAsync(string disputeId, CancellationToken cancellationToken = default)
        {
            var dispute = await _context.Disputes
                .Include(d => d.Evidence)
                .Include(d => d.Messages)
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId, cancellationToken);

            if (dispute == null)
            {
                throw new ArgumentException("Dispute not found");
            }

            return MapToDto(dispute);
        }

        public async Task<DisputeListResponse> GetUserDisputesAsync(string userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var query = _context.Disputes
                .Where(d => d.FiledBy == userId || d.AgainstUserId == userId)
                .OrderByDescending(d => d.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var disputes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DisputeListItem
                {
                    DisputeId = d.DisputeId,
                    BookingId = d.BookingId,
                    Title = d.Title,
                    Category = d.Category.ToString(),
                    Status = d.Status.ToString(),
                    Priority = d.Priority.ToString(),
                    FiledBy = d.FiledBy,
                    AssignedTo = d.AssignedTo,
                    CreatedAt = d.CreatedAt,
                    ResolvedAt = d.ResolvedAt,
                    EvidenceCount = d.Evidence.Count,
                    MessageCount = d.Messages.Count
                })
                .ToListAsync(cancellationToken);

            return new DisputeListResponse
            {
                Disputes = disputes,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DisputeListResponse> GetAdminDisputeQueueAsync(int page = 1, int pageSize = 20, int? status = null, int? priority = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Disputes.AsQueryable();

            if (status.HasValue)
            {
                var filterStatus = (DisputeStatus)status.Value;
                query = query.Where(d => d.Status == filterStatus);
            }

            if (priority.HasValue)
            {
                var filterPriority = (DisputePriority)priority.Value;
                query = query.Where(d => d.Priority == filterPriority);
            }

            var orderedQuery = query.OrderByDescending(d => d.Priority).ThenByDescending(d => d.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);
            var disputes = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DisputeListItem
                {
                    DisputeId = d.DisputeId,
                    BookingId = d.BookingId,
                    Title = d.Title,
                    Category = d.Category.ToString(),
                    Status = d.Status.ToString(),
                    Priority = d.Priority.ToString(),
                    FiledBy = d.FiledBy,
                    AssignedTo = d.AssignedTo,
                    CreatedAt = d.CreatedAt,
                    ResolvedAt = d.ResolvedAt,
                    EvidenceCount = d.Evidence.Count,
                    MessageCount = d.Messages.Count
                })
                .ToListAsync(cancellationToken);

            return new DisputeListResponse
            {
                Disputes = disputes,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DisputeEvidenceDto> SubmitEvidenceAsync(string userId, string disputeId, SubmitEvidenceRequest request, CancellationToken cancellationToken = default)
        {
            var dispute = await _context.Disputes
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId, cancellationToken);

            if (dispute == null)
            {
                throw new ArgumentException("Dispute not found");
            }

            if (dispute.Status == DisputeStatus.Closed || dispute.Status == DisputeStatus.Resolved)
            {
                throw new InvalidOperationException("Cannot submit evidence to a closed or resolved dispute");
            }

            var evidence = new DisputeEvidence
            {
                EvidenceId = Guid.NewGuid().ToString(),
                DisputeId = disputeId,
                SubmittedBy = userId,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileType = request.FileType,
                Description = request.Description,
                SubmittedAt = DateTime.UtcNow
            };

            _context.DisputeEvidence.Add(evidence);
            dispute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Evidence {EvidenceId} submitted for dispute {DisputeId} by user {UserId}", evidence.EvidenceId, disputeId, userId);

            return new DisputeEvidenceDto
            {
                EvidenceId = evidence.EvidenceId,
                SubmittedBy = evidence.SubmittedBy,
                FileName = evidence.FileName,
                FileUrl = evidence.FileUrl,
                FileType = evidence.FileType,
                Description = evidence.Description,
                SubmittedAt = evidence.SubmittedAt
            };
        }

        public async Task<DisputeMessageDto> AddMessageAsync(string userId, string disputeId, DisputeMessageRequest request, CancellationToken cancellationToken = default)
        {
            var dispute = await _context.Disputes
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId, cancellationToken);

            if (dispute == null)
            {
                throw new ArgumentException("Dispute not found");
            }

            if (dispute.Status == DisputeStatus.Closed || dispute.Status == DisputeStatus.Resolved)
            {
                throw new InvalidOperationException("Cannot add messages to a closed or resolved dispute");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            var message = new DisputeMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                DisputeId = disputeId,
                SenderId = userId,
                SenderName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                Content = request.Content,
                IsAdminMessage = dispute.AssignedTo == userId,
                SentAt = DateTime.UtcNow
            };

            _context.DisputeMessages.Add(message);
            dispute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new DisputeMessageDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                Content = message.Content,
                IsAdminMessage = message.IsAdminMessage,
                SentAt = message.SentAt
            };
        }

        public async Task<bool> AssignDisputeAsync(string adminId, string disputeId, CancellationToken cancellationToken = default)
        {
            var dispute = await _context.Disputes
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId, cancellationToken);

            if (dispute == null)
            {
                return false;
            }

            dispute.AssignedTo = adminId;
            dispute.Status = DisputeStatus.UnderReview;
            dispute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Dispute {DisputeId} assigned to admin {AdminId}", disputeId, adminId);

            return true;
        }

        public async Task<DisputeDto> ResolveDisputeAsync(string adminId, string disputeId, ResolveDisputeRequest request, CancellationToken cancellationToken = default)
        {
            var dispute = await _context.Disputes
                .Include(d => d.Evidence)
                .Include(d => d.Messages)
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId, cancellationToken);

            if (dispute == null)
            {
                throw new ArgumentException("Dispute not found");
            }

            dispute.Status = DisputeStatus.Resolved;
            dispute.Resolution = request.Resolution;
            dispute.RefundAmount = request.RefundAmount;
            dispute.ResolvedAt = DateTime.UtcNow;
            dispute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Dispute {DisputeId} resolved by admin {AdminId}", disputeId, adminId);

            return MapToDto(dispute);
        }

        public async Task<bool> EscalateDisputeAsync(string adminId, string disputeId, CancellationToken cancellationToken = default)
        {
            var dispute = await _context.Disputes
                .FirstOrDefaultAsync(d => d.DisputeId == disputeId, cancellationToken);

            if (dispute == null)
            {
                return false;
            }

            dispute.Status = DisputeStatus.Escalated;
            dispute.Priority = DisputePriority.Urgent;
            dispute.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Dispute {DisputeId} escalated by admin {AdminId}", disputeId, adminId);

            return true;
        }

        public async Task<DisputeStatsDto> GetDisputeStatsAsync(CancellationToken cancellationToken = default)
        {
            var stats = new DisputeStatsDto
            {
                OpenCount = await _context.Disputes.CountAsync(d => d.Status == DisputeStatus.Open, cancellationToken),
                UnderReviewCount = await _context.Disputes.CountAsync(d => d.Status == DisputeStatus.UnderReview, cancellationToken),
                ResolvedCount = await _context.Disputes.CountAsync(d => d.Status == DisputeStatus.Resolved, cancellationToken)
            };

            var resolvedDisputes = await _context.Disputes
                .Where(d => d.Status == DisputeStatus.Resolved && d.ResolvedAt.HasValue)
                .Select(d => EF.Functions.DateDiffDay(d.CreatedAt, d.ResolvedAt.Value))
                .ToListAsync(cancellationToken);

            stats.AverageResolutionDays = resolvedDisputes.Any()
                ? resolvedDisputes.Average(d => (double)d)
                : 0;

            return stats;
        }

        private static DisputeDto MapToDto(Dispute dispute)
        {
            return new DisputeDto
            {
                DisputeId = dispute.DisputeId,
                BookingId = dispute.BookingId,
                FiledBy = dispute.FiledBy,
                AgainstUserId = dispute.AgainstUserId,
                Title = dispute.Title,
                Description = dispute.Description,
                Category = dispute.Category.ToString(),
                Status = dispute.Status.ToString(),
                Priority = dispute.Priority.ToString(),
                AssignedTo = dispute.AssignedTo,
                Resolution = dispute.Resolution,
                RefundAmount = dispute.RefundAmount,
                CreatedAt = dispute.CreatedAt,
                UpdatedAt = dispute.UpdatedAt,
                ResolvedAt = dispute.ResolvedAt,
                Evidence = dispute.Evidence?.Select(e => new DisputeEvidenceDto
                {
                    EvidenceId = e.EvidenceId,
                    SubmittedBy = e.SubmittedBy,
                    FileName = e.FileName,
                    FileUrl = e.FileUrl,
                    FileType = e.FileType,
                    Description = e.Description,
                    SubmittedAt = e.SubmittedAt
                }).ToList() ?? new System.Collections.Generic.List<DisputeEvidenceDto>(),
                Messages = dispute.Messages?.Select(m => new DisputeMessageDto
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    SenderName = m.SenderName,
                    Content = m.Content,
                    IsAdminMessage = m.IsAdminMessage,
                    SentAt = m.SentAt
                }).ToList() ?? new System.Collections.Generic.List<DisputeMessageDto>()
            };
        }
    }
}
