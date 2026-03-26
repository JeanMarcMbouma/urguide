using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.PushNotifications;

namespace UrGuide.Data.Configurations
{
    public class DeviceRegistrationConfiguration : IEntityTypeConfiguration<DeviceRegistration>
    {
        public void Configure(EntityTypeBuilder<DeviceRegistration> builder)
        {
            builder.ToTable("device_registrations", Constants.Schema);

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(d => d.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(d => d.DeviceToken)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(d => d.Platform)
                .IsRequired();

            builder.Property(d => d.DeviceName)
                .HasMaxLength(256);

            builder.Property(d => d.AppVersion)
                .HasMaxLength(64);

            builder.Property(d => d.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(d => d.RegisteredAt)
                .IsRequired();

            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.UserId);
            builder.HasIndex(d => d.IsActive);
            builder.HasIndex(d => new { d.UserId, d.DeviceToken }).IsUnique();
        }
    }
}
