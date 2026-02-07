using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Payments;

namespace UrGuide.Data.Configurations
{
    public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
    {
        public void Configure(EntityTypeBuilder<Payout> builder)
        {
            builder.ToTable("payouts", Constants.Schema);

            builder.HasKey(p => p.PayoutId);

            builder.Property(p => p.PayoutId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.GuideId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.StripePayoutId)
                .HasMaxLength(100);

            builder.Property(p => p.StripeAccountId)
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.RequestedAt)
                .IsRequired();

            builder.Property(p => p.ProcessedAt);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.Property(p => p.FailureReason)
                .HasMaxLength(500);

            builder.HasOne(p => p.Guide)
                .WithMany()
                .HasForeignKey(p => p.GuideId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.GuideId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.RequestedAt);
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
