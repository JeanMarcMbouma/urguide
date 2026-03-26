using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrGuide.Data.Entities.Event;

namespace UrGuide.Data.Configurations
{
    class AuditEventConfiguration : IEntityTypeConfiguration<AuditEvent>
    {
        public void Configure(EntityTypeBuilder<AuditEvent> builder)
        {
            builder.ToTable("Audit_Events", Constants.Schema);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(600);
            builder.Property(x => x.ReferenceId).HasMaxLength(500);
            var converter = new ValueConverter<EventCodes, int>(
                v => (int)v,
                v => (EventCodes)v);
            builder.Property(x => x.EventCode).HasConversion(converter).IsRequired();
            builder.Property(x => x.Created).IsRequired().HasColumnType("datetime2");
            builder.Property(x => x.IpAddress).HasMaxLength(45);
            builder.Property(x => x.UserAgent).HasMaxLength(500);
            builder.Property(x => x.Details).HasMaxLength(4000);
            builder.Property(x => x.Category).HasMaxLength(100);
            var severityConverter = new ValueConverter<AuditSeverity, int>(
                v => (int)v,
                v => (AuditSeverity)v);
            builder.Property(x => x.Severity).HasConversion(severityConverter).HasDefaultValue(AuditSeverity.Info);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Created);
            builder.HasIndex(x => x.EventCode);
            builder.HasIndex(x => x.Category);
        }
    }
}
