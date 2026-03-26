using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Premium;

namespace UrGuide.Data.Configurations
{
    class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.ToTable("subscription_plans", Constants.Schema);
            builder.HasKey(x => x.SubscriptionPlanId);
            builder.Property(x => x.SubscriptionPlanId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.Tier);
            builder.Property(x => x.BillingCycle);
            builder.Property(x => x.Price).HasPrecision(18, 2);
            builder.Property(x => x.PlatformFeePercentage).HasPrecision(5, 2);
            builder.Property(x => x.SearchRankingBoost).HasDefaultValue(0);
            builder.Property(x => x.MaxGroupSize).HasDefaultValue(3);
            builder.Property(x => x.IsActive).HasDefaultValue(true);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasMany(x => x.GuideSubscriptions)
                .WithOne(x => x.SubscriptionPlan)
                .HasForeignKey(x => x.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
