using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Data
{
    public class UrGuideAuthContext : IdentityDbContext<UrGuideUser>
    {
        public UrGuideAuthContext(DbContextOptions<UrGuideAuthContext> options) : base(options)
        {
        }
        
        public DbSet<PasskeyCredential> PasskeyCredentials { get; set; }
        public DbSet<SocialLoginProvider> SocialLoginProviders { get; set; }
        public DbSet<SocialLoginAuditLog> SocialLoginAuditLogs { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configure PasskeyCredential entity
            builder.Entity<PasskeyCredential>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.PasskeyCredentials)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.FriendlyName).HasMaxLength(100);
            });

            // Configure SocialLoginProvider entity
            builder.Entity<SocialLoginProvider>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(u => u.SocialLoginProviders)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Provider).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ProviderKey).HasMaxLength(256).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.DisplayName).HasMaxLength(256);
                entity.Property(e => e.AvatarUrl).HasMaxLength(1024);
                entity.HasIndex(e => new { e.Provider, e.ProviderKey }).IsUnique();
                entity.HasIndex(e => new { e.UserId, e.Provider }).IsUnique();
            });

            // Configure SocialLoginAuditLog entity
            builder.Entity<SocialLoginAuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Provider).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Action).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Details).HasMaxLength(1000);
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Timestamp);
            });
        }
    }
}
