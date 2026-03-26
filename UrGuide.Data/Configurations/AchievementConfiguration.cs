using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("achievements", Constants.Schema);
            builder.HasKey(x => x.AchievementId);
            builder.Property(x => x.AchievementId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.IconUrl).HasMaxLength(Constants.ImageUrlMaxLength);
            builder.Property(x => x.Category).HasMaxLength(100);
            builder.Property(x => x.ThresholdValue);
            builder.Property(x => x.PointsReward).HasDefaultValue(0);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedAt);
            builder.HasMany(x => x.UserAchievements)
                .WithOne(x => x.Achievement)
                .HasForeignKey(x => x.AchievementId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
