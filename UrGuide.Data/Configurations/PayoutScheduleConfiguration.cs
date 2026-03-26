using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Financial;

namespace UrGuide.Data.Configurations
{
    class PayoutScheduleConfiguration : IEntityTypeConfiguration<PayoutSchedule>
    {
        public void Configure(EntityTypeBuilder<PayoutSchedule> builder)
        {
            builder.ToTable("payout_schedules", Constants.Schema);
            builder.HasKey(x => x.PayoutScheduleId);
            builder.Property(x => x.PayoutScheduleId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.GuideId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Frequency);
            builder.Property(x => x.MinimumAmount).HasPrecision(18, 2);
            builder.Property(x => x.NextPayoutDate);
            builder.Property(x => x.LastPayoutDate);
            builder.Property(x => x.Status);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasIndex(x => x.GuideId);
            builder.HasIndex(x => x.Status);
        }
    }
}
