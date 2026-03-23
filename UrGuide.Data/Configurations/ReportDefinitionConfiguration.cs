using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Reports;

namespace UrGuide.Data.Configurations
{
    public class ReportDefinitionConfiguration : IEntityTypeConfiguration<ReportDefinition>
    {
        public void Configure(EntityTypeBuilder<ReportDefinition> builder)
        {
            builder.ToTable("report_definitions", Constants.Schema);

            builder.HasKey(r => r.ReportId);

            builder.Property(r => r.ReportId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.Name)
                .HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();

            builder.Property(r => r.Description)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(r => r.Type)
                .IsRequired();

            builder.Property(r => r.RequestedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(r => r.Format)
                .IsRequired();

            builder.Property(r => r.ParametersJson)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(r => r.Status)
                .IsRequired();

            builder.Property(r => r.FileUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(r => r.ErrorMessage)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasIndex(r => r.RequestedBy);
            builder.HasIndex(r => r.Status);
            builder.HasIndex(r => r.CreatedAt);
        }
    }
}
