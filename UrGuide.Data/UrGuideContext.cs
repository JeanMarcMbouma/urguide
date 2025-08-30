using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UrGuide.Data.Entities.Event;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Entities.Regions;
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
        }
    }
}
