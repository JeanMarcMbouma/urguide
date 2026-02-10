using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Webhooks;

namespace UrGuide.Data.Configurations
{
    public class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
    {
        public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
        {
            builder.ToTable("webhook_deliveries", Constants.Schema);

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.WebhookSubscriptionId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.Event)
                .IsRequired();

            builder.Property(d => d.Payload)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(d => d.Signature)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.AttemptCount)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(d => d.MaxAttempts)
                .HasDefaultValue(5)
                .IsRequired();

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.DeliveredAt);

            builder.Property(d => d.NextRetryAt);

            builder.Property(d => d.ResponseStatusCode);

            builder.Property(d => d.ResponseBody)
                .HasMaxLength(4000);

            builder.Property(d => d.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasOne(d => d.WebhookSubscription)
                .WithMany()
                .HasForeignKey(d => d.WebhookSubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.WebhookSubscriptionId);
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.CreatedAt);
            builder.HasIndex(d => d.NextRetryAt);
        }
    }
}
