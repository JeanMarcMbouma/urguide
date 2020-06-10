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
        }
    }
}
