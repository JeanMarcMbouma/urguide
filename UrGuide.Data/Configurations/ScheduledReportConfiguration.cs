using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Reports;

namespace UrGuide.Data.Configurations
{
    public class ScheduledReportConfiguration : IEntityTypeConfiguration<ScheduledReport>
    {
        public void Configure(EntityTypeBuilder<ScheduledReport> builder)
        {
            builder.ToTable("scheduled_reports", Constants.Schema);

            builder.HasKey(s => s.ScheduleId);

            builder.Property(s => s.ScheduleId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(s => s.Name)
                .HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();

            builder.Property(s => s.ReportType)
                .IsRequired();

            builder.Property(s => s.Format)
                .IsRequired();

            builder.Property(s => s.ParametersJson)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(s => s.Frequency)
                .IsRequired();

            builder.Property(s => s.EmailRecipients)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(s => s.IsActive)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasIndex(s => s.UserId);
            builder.HasIndex(s => s.IsActive);
            builder.HasIndex(s => s.NextRunAt);
        }
    }
}
