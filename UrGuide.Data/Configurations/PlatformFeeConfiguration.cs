using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Payments;

namespace UrGuide.Data.Configurations
{
    public class PlatformFeeConfiguration : IEntityTypeConfiguration<PlatformFee>
    {
        public void Configure(EntityTypeBuilder<PlatformFee> builder)
        {
            builder.ToTable("platform_fees", Constants.Schema);

            builder.HasKey(pf => pf.FeeId);

            builder.Property(pf => pf.FeeId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(pf => pf.PaymentId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(pf => pf.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(pf => pf.Percentage)
                .HasPrecision(5, 4)
                .IsRequired();

            builder.Property(pf => pf.MembershipTier)
                .IsRequired();

            builder.Property(pf => pf.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(pf => pf.CreatedAt)
                .IsRequired();

            builder.HasOne(pf => pf.Payment)
                .WithMany()
                .HasForeignKey(pf => pf.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pf => pf.PaymentId);
            builder.HasIndex(pf => pf.MembershipTier);
            builder.HasIndex(pf => pf.CreatedAt);
        }
    }
}
