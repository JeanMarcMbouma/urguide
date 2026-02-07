using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Payments;

namespace UrGuide.Data.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            builder.ToTable("refunds", Constants.Schema);

            builder.HasKey(r => r.RefundId);

            builder.Property(r => r.RefundId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.PaymentId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(r => r.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.StripeRefundId)
                .HasMaxLength(100);

            builder.Property(r => r.Reason)
                .HasMaxLength(500);

            builder.Property(r => r.RequestedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(r => r.RequestedAt)
                .IsRequired();

            builder.Property(r => r.ProcessedAt);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.UpdatedAt)
                .IsRequired();

            builder.Property(r => r.FailureReason)
                .HasMaxLength(500);

            builder.HasOne(r => r.Payment)
                .WithMany()
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.RequestedByUser)
                .WithMany()
                .HasForeignKey(r => r.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.PaymentId);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.RequestedAt);
            builder.HasIndex(r => r.CreatedAt);
        }
    }
}
