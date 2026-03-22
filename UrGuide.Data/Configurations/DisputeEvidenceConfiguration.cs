using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Disputes;

namespace UrGuide.Data.Configurations
{
    public class DisputeEvidenceConfiguration : IEntityTypeConfiguration<DisputeEvidence>
    {
        public void Configure(EntityTypeBuilder<DisputeEvidence> builder)
        {
            builder.ToTable("dispute_evidence", Constants.Schema);

            builder.HasKey(e => e.EvidenceId);

            builder.Property(e => e.EvidenceId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.DisputeId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.SubmittedBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.FileUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength)
                .IsRequired();

            builder.Property(e => e.FileType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(e => e.SubmittedAt)
                .IsRequired();

            builder.HasIndex(e => e.DisputeId);
            builder.HasIndex(e => e.SubmittedBy);
        }
    }
}
