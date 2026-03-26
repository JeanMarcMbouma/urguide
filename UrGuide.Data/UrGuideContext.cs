using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UrGuide.Data.Entities.Event;
using UrGuide.Data.Entities.Messages;
using UrGuide.Data.Entities.Payments;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Entities.Referrals;
using UrGuide.Data.Entities.Regions;
using UrGuide.Data.Entities.Search;
using UrGuide.Data.Entities.Shared;
using UrGuide.Data.Entities.Tour;
using UrGuide.Data.Entities.Users;
using UrGuide.Data.Entities.Media;
using UrGuide.Data.Entities.Webhooks;
using UrGuide.Data.Entities.Disputes;
using UrGuide.Data.Entities.Recommendations;
using UrGuide.Data.Entities.Email;
using UrGuide.Data.Entities.Financial;
using UrGuide.Data.Entities.Gamification;
using UrGuide.Data.Entities.Premium;
using UrGuide.Data.Entities.Reports;

namespace UrGuide.Data
{
    public class UrGuideContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<ImageCatalog> ImageCatalogs { get; set; }
        public virtual DbSet<AuditEvent> AuditEvents { get; set; }
        public virtual DbSet<TourRequest> TourRequests { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        
        // Payment entities
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public virtual DbSet<Payout> Payouts { get; set; }
        public virtual DbSet<Refund> Refunds { get; set; }
        public virtual DbSet<PlatformFee> PlatformFees { get; set; }
        
        // Data export entities
        public virtual DbSet<DataExportRequest> DataExportRequests { get; set; }
        
        // Search entities
        public virtual DbSet<SearchAnalytics> SearchAnalytics { get; set; }
        
        // Webhook entities
        public virtual DbSet<WebhookSubscription> WebhookSubscriptions { get; set; }
        public virtual DbSet<WebhookDelivery> WebhookDeliveries { get; set; }

        // Guide availability entities
        public virtual DbSet<GuideBlockedDate> GuideBlockedDates { get; set; }
        public virtual DbSet<GuideRecurringPattern> GuideRecurringPatterns { get; set; }
        public virtual DbSet<GuideGoogleCalendarToken> GuideGoogleCalendarTokens { get; set; }

        // Guide verification entities
        public virtual DbSet<GuideVerificationSubmission> GuideVerificationSubmissions { get; set; }
        public virtual DbSet<GuideVerificationDocument> GuideVerificationDocuments { get; set; }

        // Messaging entities
        public virtual DbSet<ConversationEntity> Conversations { get; set; }
        public virtual DbSet<MessageEntity> MessageEntities { get; set; }
        public virtual DbSet<FileAttachment> FileAttachments { get; set; }

        // Review moderation entities
        public virtual DbSet<ReviewFlag> ReviewFlags { get; set; }
        public virtual DbSet<ReviewModerationAction> ReviewModerationActions { get; set; }

        // Tour template entities
        public virtual DbSet<TourTemplate> TourTemplates { get; set; }

        // Referral entities
        public virtual DbSet<ReferralCode> ReferralCodes { get; set; }
        public virtual DbSet<Referral> Referrals { get; set; }

        // Image processing entities
        public virtual DbSet<ProcessedImage> ProcessedImages { get; set; }

        // Dispute entities
        public virtual DbSet<Dispute> Disputes { get; set; }
        public virtual DbSet<DisputeEvidence> DisputeEvidence { get; set; }
        public virtual DbSet<DisputeMessage> DisputeMessages { get; set; }

        // Recommendation entities
        public virtual DbSet<UserPreference> UserPreferences { get; set; }
        public virtual DbSet<TourInteraction> TourInteractions { get; set; }
        public virtual DbSet<RecommendationLog> RecommendationLogs { get; set; }

        // Report entities
        public virtual DbSet<ReportDefinition> ReportDefinitions { get; set; }
        public virtual DbSet<ScheduledReport> ScheduledReports { get; set; }

        // Financial entities
        public virtual DbSet<CoinWallet> CoinWallets { get; set; }
        public virtual DbSet<CoinTransaction> CoinTransactions { get; set; }
        public virtual DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
        public virtual DbSet<PayoutSchedule> PayoutSchedules { get; set; }

        // Gamification entities
        public virtual DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; }
        public virtual DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }
        public virtual DbSet<Badge> Badges { get; set; }
        public virtual DbSet<UserBadge> UserBadges { get; set; }
        public virtual DbSet<LotteryDraw> LotteryDraws { get; set; }
        public virtual DbSet<LotteryEntry> LotteryEntries { get; set; }
        public virtual DbSet<Achievement> Achievements { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }

        // Premium entities
        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public virtual DbSet<GuideSubscription> GuideSubscriptions { get; set; }
        public virtual DbSet<VisibilityBoost> VisibilityBoosts { get; set; }
        public virtual DbSet<Advertisement> Advertisements { get; set; }

        // Email template entities
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailTemplateVersion> EmailTemplateVersions { get; set; }

        public UrGuideContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new Configurations.CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ImageCatalogConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PostConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AuditEventConfiguration());


            modelBuilder.ApplyConfiguration(new Configurations.CampainConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CurrencyConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PaymentMethodConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RegionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TimelineConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TourConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TourRequestConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AuthorConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.SubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.BookingConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.BalanceConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ReviewConfiguration());
            
            // Payment configurations
            modelBuilder.ApplyConfiguration(new Configurations.PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PaymentTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PayoutConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RefundConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PlatformFeeConfiguration());
            
            // Data export configurations
            modelBuilder.ApplyConfiguration(new Configurations.DataExportRequestConfiguration());
            
            // Search configurations
            modelBuilder.ApplyConfiguration(new Configurations.SearchAnalyticsConfiguration());
            
            // Webhook configurations
            modelBuilder.ApplyConfiguration(new Configurations.WebhookSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.WebhookDeliveryConfiguration());

            // Guide availability configurations
            modelBuilder.ApplyConfiguration(new Configurations.GuideBlockedDateConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.GuideRecurringPatternConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.GuideGoogleCalendarTokenConfiguration());

            // Guide verification configurations
            modelBuilder.ApplyConfiguration(new Configurations.GuideVerificationSubmissionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.GuideVerificationDocumentConfiguration());

            // Messaging configurations
            modelBuilder.ApplyConfiguration(new Configurations.ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.MessageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.FileAttachmentConfiguration());

            // Review moderation configurations
            modelBuilder.ApplyConfiguration(new Configurations.ReviewFlagConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ReviewModerationActionConfiguration());

            // Tour template configurations
            modelBuilder.ApplyConfiguration(new Configurations.TourTemplateConfiguration());

            // Referral configurations
            modelBuilder.ApplyConfiguration(new Configurations.ReferralCodeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ReferralConfiguration());

            // Image processing configurations
            modelBuilder.ApplyConfiguration(new Configurations.ProcessedImageConfiguration());

            // Dispute configurations
            modelBuilder.ApplyConfiguration(new Configurations.DisputeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.DisputeEvidenceConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.DisputeMessageConfiguration());

            // Recommendation configurations
            modelBuilder.ApplyConfiguration(new Configurations.UserPreferenceConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.TourInteractionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.RecommendationLogConfiguration());

            // Report configurations
            modelBuilder.ApplyConfiguration(new Configurations.ReportDefinitionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.ScheduledReportConfiguration());

            // Email template configurations
            modelBuilder.ApplyConfiguration(new Configurations.EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.EmailTemplateVersionConfiguration());

            // Financial configurations
            modelBuilder.ApplyConfiguration(new Configurations.CoinWalletConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CoinTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.WithdrawalRequestConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.PayoutScheduleConfiguration());

            // Gamification configurations
            modelBuilder.ApplyConfiguration(new Configurations.LoyaltyAccountConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.LoyaltyTransactionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.BadgeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserBadgeConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.LotteryDrawConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.LotteryEntryConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AchievementConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.UserAchievementConfiguration());

            // Premium configurations
            modelBuilder.ApplyConfiguration(new Configurations.SubscriptionPlanConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.GuideSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.VisibilityBoostConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.AdvertisementConfiguration());
        }
    }
}
