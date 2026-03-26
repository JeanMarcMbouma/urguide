using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Premium;

namespace UrGuide.Data.Configurations
{
    class GuideSubscriptionConfiguration : IEntityTypeConfiguration<GuideSubscription>
    {
        public void Configure(EntityTypeBuilder<GuideSubscription> builder)
        {
            builder.ToTable("guide_subscriptions", Constants.Schema);
            builder.HasKey(x => x.GuideSubscriptionId);
            builder.Property(x => x.GuideSubscriptionId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.GuideId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.SubscriptionPlanId).IsRequired();
            builder.Property(x => x.Status);
            builder.Property(x => x.StartDate);
            builder.Property(x => x.EndDate);
            builder.Property(x => x.AutoRenew).HasDefaultValue(true);
            builder.Property(x => x.StripeSubscriptionId).HasMaxLength(450);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasIndex(x => x.GuideId);
            builder.HasIndex(x => x.Status);
        }
    }
}
