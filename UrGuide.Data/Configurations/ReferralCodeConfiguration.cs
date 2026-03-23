using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Referrals;

namespace UrGuide.Data.Configurations
{
    class ReferralCodeConfiguration : IEntityTypeConfiguration<ReferralCode>
    {
        public void Configure(EntityTypeBuilder<ReferralCode> builder)
        {
            builder.ToTable("referral_codes", Constants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasDefaultValueSql(Constants.GuidFn);

            builder.Property(x => x.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(x => x.Code)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.TotalReferrals)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(x => x.TotalEarnings)
                .HasPrecision(18, 2)
                .HasDefaultValue(0m)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
