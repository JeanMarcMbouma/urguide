using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using UrGuide.Data.Entities.Messages;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data
{
    public class UrGuideContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Notification> Messages { get; set; }

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
            modelBuilder.ApplyConfiguration(new Configurations.NotificationConfiguration());
        }
    }
}
