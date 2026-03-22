using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Referrals;
using UrGuide.Model.Referrals;

namespace UrGuide.Services.Referrals
{
    public class ReferralService : IReferralService
    {
        private const decimal DefaultRewardAmount = 10.00m;
        private const string DefaultCurrencyCode = "USD";

        private readonly UrGuideContext _context;
        private readonly ILogger<ReferralService> _logger;

        public ReferralService(UrGuideContext context, ILogger<ReferralService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Outcome<ReferralCodeDto>> GenerateReferralCodeAsync(string userId, ReferralCodeType type)
        {
            try
            {
                var existing = await _context.ReferralCodes
                    .FirstOrDefaultAsync(rc => rc.UserId == userId && rc.Type == type && rc.IsActive);

                if (existing != null)
                    return Result.Of(MapToCodeDto(existing));

                var code = await GenerateUniqueCodeAsync();

                var referralCode = new ReferralCode
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Code = code,
                    Type = type,
                    TotalReferrals = 0,
                    TotalEarnings = 0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ReferralCodes.Add(referralCode);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Referral code generated for user {UserId}: {Code}", userId, code);

                return Result.Of(MapToCodeDto(referralCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating referral code for user {UserId}", userId);
                return Result.Of<ReferralCodeDto>().WithErrors("An error occurred while generating the referral code");
            }
        }

        public async Task<Outcome<ReferralCodeDto>> GetUserReferralCodeAsync(string userId)
        {
            try
            {
                var referralCode = await _context.ReferralCodes
                    .FirstOrDefaultAsync(rc => rc.UserId == userId && rc.IsActive);

                if (referralCode == null)
                    return Result.Of<ReferralCodeDto>().WithErrors("No referral code found for this user");

                return Result.Of(MapToCodeDto(referralCode));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referral code for user {UserId}", userId);
                return Result.Of<ReferralCodeDto>().WithErrors("An error occurred while retrieving the referral code");
            }
        }

        public async Task<Outcome<bool>> ApplyReferralCodeAsync(string newUserId, string code)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(code))
                    return Result.Of(false).WithErrors("Referral code is required");

                var referralCode = await _context.ReferralCodes
                    .FirstOrDefaultAsync(rc => rc.Code == code && rc.IsActive);

                if (referralCode == null)
                    return Result.Of(false).WithErrors("Invalid or inactive referral code");

                if (referralCode.UserId == newUserId)
                    return Result.Of(false).WithErrors("You cannot use your own referral code");

                var alreadyReferred = await _context.Referrals
                    .AnyAsync(r => r.ReferredUserId == newUserId);

                if (alreadyReferred)
                    return Result.Of(false).WithErrors("This user has already been referred");

                var referral = new Referral
                {
                    Id = Guid.NewGuid().ToString(),
                    ReferralCodeId = referralCode.Id,
                    ReferrerId = referralCode.UserId,
                    ReferredUserId = newUserId,
                    Status = ReferralStatus.Pending,
                    RewardAmount = DefaultRewardAmount,
                    CurrencyCode = DefaultCurrencyCode,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Referrals.Add(referral);
                referralCode.TotalReferrals++;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Referral code {Code} applied by user {NewUserId}", code, newUserId);

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying referral code {Code} for user {NewUserId}", code, newUserId);
                return Result.Of(false).WithErrors("An error occurred while applying the referral code");
            }
        }

        public async Task<Outcome<ReferralDashboardDto>> GetReferralDashboardAsync(string userId)
        {
            try
            {
                var referralCode = await _context.ReferralCodes
                    .FirstOrDefaultAsync(rc => rc.UserId == userId && rc.IsActive);

                if (referralCode == null)
                    return Result.Of<ReferralDashboardDto>().WithErrors("No referral code found. Generate one first.");

                var recentReferrals = await _context.Referrals
                    .Where(r => r.ReferralCodeId == referralCode.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(10)
                    .Select(r => new ReferralDto
                    {
                        Id = r.Id,
                        ReferredUserId = r.ReferredUserId,
                        Status = r.Status,
                        RewardAmount = r.RewardAmount,
                        CreatedAt = r.CreatedAt,
                        CompletedAt = r.CompletedAt
                    })
                    .ToListAsync();

                var pendingRewards = await _context.Referrals
                    .Where(r => r.ReferralCodeId == referralCode.Id && r.Status == ReferralStatus.Completed)
                    .SumAsync(r => r.RewardAmount);

                var dashboard = new ReferralDashboardDto
                {
                    Code = referralCode.Code,
                    TotalReferrals = referralCode.TotalReferrals,
                    TotalEarnings = referralCode.TotalEarnings,
                    PendingRewards = pendingRewards,
                    RecentReferrals = recentReferrals
                };

                return Result.Of(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referral dashboard for user {UserId}", userId);
                return Result.Of<ReferralDashboardDto>().WithErrors("An error occurred while retrieving the referral dashboard");
            }
        }

        public async Task<Outcome<bool>> CompleteReferralAsync(string referredUserId)
        {
            try
            {
                var referral = await _context.Referrals
                    .Include(r => r.ReferralCode)
                    .FirstOrDefaultAsync(r => r.ReferredUserId == referredUserId && r.Status == ReferralStatus.Pending);

                if (referral == null)
                    return Result.Of(false).WithErrors("No pending referral found for this user");

                referral.Status = ReferralStatus.Completed;
                referral.CompletedAt = DateTime.UtcNow;

                if (referral.ReferralCode != null)
                {
                    referral.ReferralCode.TotalEarnings += referral.RewardAmount;
                    referral.Status = ReferralStatus.Rewarded;
                    referral.RewardedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Referral completed and rewarded for referred user {ReferredUserId}", referredUserId);

                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing referral for user {ReferredUserId}", referredUserId);
                return Result.Of(false).WithErrors("An error occurred while completing the referral");
            }
        }

        public async Task<Outcome<List<ReferralDto>>> GetReferralsByCodeAsync(string code, int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 20;

                var referralCode = await _context.ReferralCodes
                    .FirstOrDefaultAsync(rc => rc.Code == code);

                if (referralCode == null)
                    return Result.Of<List<ReferralDto>>().WithErrors("Referral code not found");

                var referrals = await _context.Referrals
                    .Where(r => r.ReferralCodeId == referralCode.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new ReferralDto
                    {
                        Id = r.Id,
                        ReferredUserId = r.ReferredUserId,
                        Status = r.Status,
                        RewardAmount = r.RewardAmount,
                        CreatedAt = r.CreatedAt,
                        CompletedAt = r.CompletedAt
                    })
                    .ToListAsync();

                return Result.Of(referrals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving referrals for code {Code}", code);
                return Result.Of<List<ReferralDto>>().WithErrors("An error occurred while retrieving referrals");
            }
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            string code;

            do
            {
                code = new string(Enumerable.Range(0, 8)
                    .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)])
                    .ToArray());
            }
            while (await _context.ReferralCodes.AnyAsync(rc => rc.Code == code));

            return code;
        }

        private static ReferralCodeDto MapToCodeDto(ReferralCode referralCode)
        {
            return new ReferralCodeDto
            {
                Code = referralCode.Code,
                Type = referralCode.Type,
                TotalReferrals = referralCode.TotalReferrals,
                TotalEarnings = referralCode.TotalEarnings,
                IsActive = referralCode.IsActive,
                CreatedAt = referralCode.CreatedAt
            };
        }
    }
}
