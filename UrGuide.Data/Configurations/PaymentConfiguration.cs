using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Payments;

namespace UrGuide.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments", Constants.Schema);

            builder.HasKey(p => p.PaymentId);

            builder.Property(p => p.PaymentId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(p => p.BookingId)
                .HasMaxLength(50);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.CurrencyCode)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.StripePaymentIntentId)
                .HasMaxLength(100);

            builder.Property(p => p.StripeCustomerId)
                .HasMaxLength(100);

            builder.Property(p => p.PaymentMethod)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.PlatformFeeAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.GuidePayout)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.Property(p => p.Metadata)
                .HasMaxLength(1000);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Booking)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Currency)
                .WithMany()
                .HasForeignKey(p => p.CurrencyCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.StripePaymentIntentId);
            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.BookingId);
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
