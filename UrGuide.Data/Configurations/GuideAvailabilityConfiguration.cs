using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    public class GuideBlockedDateConfiguration : IEntityTypeConfiguration<GuideBlockedDate>
    {
        public void Configure(EntityTypeBuilder<GuideBlockedDate> builder)
        {
            builder.ToTable("guide_blocked_dates", Constants.Schema);

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(d => d.GuideId).HasMaxLength(450).IsRequired();
            builder.Property(d => d.Date).IsRequired();
            builder.Property(d => d.Reason).HasMaxLength(500);
            builder.Property(d => d.CreatedAt).IsRequired();

            builder.HasOne(d => d.Guide)
                .WithMany()
                .HasForeignKey(d => d.GuideId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => new { d.GuideId, d.Date }).IsUnique();
        }
    }

    public class GuideRecurringPatternConfiguration : IEntityTypeConfiguration<GuideRecurringPattern>
    {
        public void Configure(EntityTypeBuilder<GuideRecurringPattern> builder)
        {
            builder.ToTable("guide_recurring_patterns", Constants.Schema);

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(p => p.GuideId).HasMaxLength(450).IsRequired();
            builder.Property(p => p.PatternType).HasMaxLength(20).IsRequired();
            builder.Property(p => p.DayOfWeek);
            builder.Property(p => p.DayOfMonth);
            builder.Property(p => p.EndDate);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt).IsRequired();

            builder.HasOne(p => p.Guide)
                .WithMany()
                .HasForeignKey(p => p.GuideId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.GuideId).IsUnique(); // One active pattern per guide
        }
    }

    public class GuideGoogleCalendarTokenConfiguration : IEntityTypeConfiguration<GuideGoogleCalendarToken>
    {
        public void Configure(EntityTypeBuilder<GuideGoogleCalendarToken> builder)
        {
            builder.ToTable("guide_google_calendar_tokens", Constants.Schema);

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(t => t.GuideId).HasMaxLength(450).IsRequired();
            builder.Property(t => t.EncryptedAccessToken).HasColumnType("nvarchar(max)").IsRequired();
            builder.Property(t => t.EncryptedRefreshToken).HasColumnType("nvarchar(max)");
            builder.Property(t => t.TokenType).HasMaxLength(50).IsRequired().HasDefaultValue("Bearer");
            builder.Property(t => t.Scope).HasMaxLength(2000).IsRequired();
            builder.Property(t => t.ExpiresAt).IsRequired();
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt).IsRequired();

            builder.HasOne(t => t.Guide)
                .WithMany()
                .HasForeignKey(t => t.GuideId)
                .OnDelete(DeleteBehavior.Cascade);

            // One token record per guide
            builder.HasIndex(t => t.GuideId).IsUnique();
        }
    }
}
