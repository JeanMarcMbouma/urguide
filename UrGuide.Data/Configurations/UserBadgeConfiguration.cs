using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
    {
        public void Configure(EntityTypeBuilder<UserBadge> builder)
        {
            builder.ToTable("user_badges", Constants.Schema);
            builder.HasKey(x => x.UserBadgeId);
            builder.Property(x => x.UserBadgeId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.BadgeId).IsRequired();
            builder.Property(x => x.EarnedAt);
            builder.HasIndex(x => new { x.UserId, x.BadgeId }).IsUnique();
        }
    }
}
