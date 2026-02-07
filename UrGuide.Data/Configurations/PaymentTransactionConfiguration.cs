using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Payments;

namespace UrGuide.Data.Configurations
{
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("payment_transactions", Constants.Schema);

            builder.HasKey(pt => pt.TransactionId);

            builder.Property(pt => pt.TransactionId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(pt => pt.PaymentId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(pt => pt.Type)
                .IsRequired();

            builder.Property(pt => pt.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(pt => pt.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(pt => pt.Description)
                .HasMaxLength(500);

            builder.Property(pt => pt.StripeTransactionId)
                .HasMaxLength(100);

            builder.Property(pt => pt.CreatedAt)
                .IsRequired();

            builder.Property(pt => pt.Metadata)
                .HasMaxLength(1000);

            builder.HasOne(pt => pt.Payment)
                .WithMany()
                .HasForeignKey(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(pt => pt.PaymentId);
            builder.HasIndex(pt => pt.Type);
            builder.HasIndex(pt => pt.CreatedAt);
        }
    }
}
