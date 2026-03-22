using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Referrals;

namespace UrGuide.Data.Configurations
{
    class ReferralConfiguration : IEntityTypeConfiguration<Referral>
    {
        public void Configure(EntityTypeBuilder<Referral> builder)
        {
            builder.ToTable("referrals", Constants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql(Constants.GuidFn);

            builder.Property(x => x.ReferralCodeId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.ReferrerId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.ReferredUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.RewardAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CompletedAt);

            builder.Property(x => x.RewardedAt);

            builder.HasOne(x => x.ReferralCode)
                .WithMany(rc => rc.Referrals)
                .HasForeignKey(x => x.ReferralCodeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ReferralCodeId);
            builder.HasIndex(x => x.ReferrerId);
            builder.HasIndex(x => x.ReferredUserId);
            builder.HasIndex(x => x.Status);
        }
    }
}
