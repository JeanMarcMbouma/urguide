using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
    {
        public void Configure(EntityTypeBuilder<UserAchievement> builder)
        {
            builder.ToTable("user_achievements", Constants.Schema);
            builder.HasKey(x => x.UserAchievementId);
            builder.Property(x => x.UserAchievementId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.AchievementId).IsRequired();
            builder.Property(x => x.Progress).HasDefaultValue(0);
            builder.Property(x => x.IsCompleted).HasDefaultValue(false);
            builder.Property(x => x.CompletedAt);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();
        }
    }
}
