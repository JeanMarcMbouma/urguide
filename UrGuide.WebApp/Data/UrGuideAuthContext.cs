using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Data
{
    public class UrGuideAuthContext : IdentityDbContext<UrGuideUser>
    {
        public UrGuideAuthContext(DbContextOptions<UrGuideAuthContext> options) : base(options)
        {
        }
        
        public DbSet<PasskeyCredential> PasskeyCredentials { get; set; }
        
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
        }
    }
}
