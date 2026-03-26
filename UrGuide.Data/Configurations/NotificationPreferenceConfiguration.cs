using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.PushNotifications;

namespace UrGuide.Data.Configurations
{
    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.ToTable("notification_preferences", Constants.Schema);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(p => p.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(p => p.PushEnabled)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.TourUpdatesEnabled)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.BookingAlertsEnabled)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.ChatMessagesEnabled)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.PromotionalEnabled)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(p => p.SystemAlertsEnabled)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.UserId).IsUnique();
        }
    }
}
