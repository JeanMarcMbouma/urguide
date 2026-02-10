using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Webhooks;
using UrGuide.Model.Webhooks;
using System.Collections.Generic;
using System.Text.Json;

namespace UrGuide.Data.Configurations
{
    public class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
    {
        public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
        {
            builder.ToTable("webhook_subscriptions", Constants.Schema);

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Id)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(w => w.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(w => w.Url)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(w => w.Secret)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(w => w.IsActive)
                .IsRequired();

            builder.Property(w => w.Description)
                .HasMaxLength(500);

            // Store events as JSON array
            builder.Property(w => w.Events)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<WebhookEvent>>(v, (JsonSerializerOptions)null))
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.UpdatedAt)
                .IsRequired();

            builder.Property(w => w.LastTriggeredAt);

            builder.Property(w => w.SuccessCount)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(w => w.FailureCount)
                .HasDefaultValue(0)
                .IsRequired();

            builder.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => w.UserId);
            builder.HasIndex(w => w.IsActive);
            builder.HasIndex(w => w.CreatedAt);
        }
    }
}
