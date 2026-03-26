using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("badges", Constants.Schema);
            builder.HasKey(x => x.BadgeId);
            builder.Property(x => x.BadgeId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.IconUrl).HasMaxLength(Constants.ImageUrlMaxLength);
            builder.Property(x => x.Tier);
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.Property(x => x.Criteria).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.ThresholdValue);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedAt);
            builder.HasMany(x => x.UserBadges)
                .WithOne(x => x.Badge)
                .HasForeignKey(x => x.BadgeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
