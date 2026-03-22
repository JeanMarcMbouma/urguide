using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Email;

namespace UrGuide.Data.Configurations
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            builder.ToTable("email_templates", Constants.Schema);

            builder.HasKey(e => e.TemplateId);

            builder.Property(e => e.TemplateId)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();

            builder.Property(e => e.Subject)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.HtmlBody)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(e => e.PlainTextBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.Category)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Language)
                .HasMaxLength(10)
                .HasDefaultValue("en")
                .IsRequired();

            builder.Property(e => e.Version)
                .HasDefaultValue(1)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(e => e.VariablesJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();

            builder.HasIndex(e => e.Name);
            builder.HasIndex(e => e.Category);
            builder.HasIndex(e => e.Language);
            builder.HasIndex(e => e.IsActive);
            builder.HasIndex(e => new { e.Name, e.Language }).IsUnique();
        }
    }
}
