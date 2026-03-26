using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.PushNotifications;

namespace UrGuide.Data.Configurations
{
    public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.ToTable("notification_templates", Constants.Schema);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Category)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(t => t.Language)
                .HasMaxLength(10)
                .HasDefaultValue("en")
                .IsRequired();

            builder.Property(t => t.Version)
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(t => t.TitleTemplate)
                .HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();

            builder.Property(t => t.BodyTemplate)
                .HasMaxLength(Constants.DescriptionMaxLength)
                .IsRequired();

            builder.Property(t => t.ImageUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(t => t.ActionUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(t => t.VariantGroup)
                .HasMaxLength(50);

            builder.Property(t => t.CreatedBy)
                .HasMaxLength(450);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired();

            // Index to look up active templates by name + language
            builder.HasIndex(t => new { t.Name, t.Language, t.IsActive });

            // Index for listing templates by category
            builder.HasIndex(t => t.Category);

            // Index for version history queries
            builder.HasIndex(t => new { t.Name, t.Language, t.Version });
        }
    }
}
