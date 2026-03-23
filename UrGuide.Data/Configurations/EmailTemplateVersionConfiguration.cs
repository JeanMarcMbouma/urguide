using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Email;

namespace UrGuide.Data.Configurations
{
    public class EmailTemplateVersionConfiguration : IEntityTypeConfiguration<EmailTemplateVersion>
    {
        public void Configure(EntityTypeBuilder<EmailTemplateVersion> builder)
        {
            builder.ToTable("email_template_versions", Constants.Schema);

            builder.HasKey(v => v.VersionId);

            builder.Property(v => v.VersionId)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(v => v.TemplateId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(v => v.VersionNumber)
                .IsRequired();

            builder.Property(v => v.Subject)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(v => v.HtmlBody)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(v => v.PlainTextBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(v => v.ChangeSummary)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(v => v.CreatedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.HasOne(v => v.Template)
                .WithMany(t => t.Versions)
                .HasForeignKey(v => v.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => v.TemplateId);
            builder.HasIndex(v => new { v.TemplateId, v.VersionNumber }).IsUnique();
        }
    }
}
