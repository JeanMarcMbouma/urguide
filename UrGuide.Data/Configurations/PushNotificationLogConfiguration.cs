using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.PushNotifications;

namespace UrGuide.Data.Configurations
{
    public class PushNotificationLogConfiguration : IEntityTypeConfiguration<PushNotificationLog>
    {
        public void Configure(EntityTypeBuilder<PushNotificationLog> builder)
        {
            builder.ToTable("push_notification_logs", Constants.Schema);

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(l => l.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(l => l.DeviceRegistrationId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(l => l.Platform)
                .IsRequired();

            builder.Property(l => l.Title)
                .HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();

            builder.Property(l => l.Body)
                .HasMaxLength(Constants.DescriptionMaxLength)
                .IsRequired();

            builder.Property(l => l.Status)
                .IsRequired();

            builder.Property(l => l.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(l => l.SentAt)
                .IsRequired();

            builder.Property(l => l.TemplateId)
                .HasMaxLength(100);

            builder.HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.DeviceRegistration)
                .WithMany()
                .HasForeignKey(l => l.DeviceRegistrationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(l => l.UserId);
            builder.HasIndex(l => l.SentAt);
            builder.HasIndex(l => l.Status);
        }
    }
}
