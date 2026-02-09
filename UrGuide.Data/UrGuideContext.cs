using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UrGuide.Data.Entities.Event;
using UrGuide.Data.Entities.Payments;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Entities.Regions;
using UrGuide.Data.Entities.Search;
using UrGuide.Data.Entities.Shared;
using UrGuide.Data.Entities.Tour;
using UrGuide.Data.Entities.Users;

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
        }
    }
}
