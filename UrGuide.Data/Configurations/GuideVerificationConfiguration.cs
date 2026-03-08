using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    public class GuideVerificationSubmissionConfiguration : IEntityTypeConfiguration<GuideVerificationSubmission>
    {
        public void Configure(EntityTypeBuilder<GuideVerificationSubmission> builder)
        {
            builder.ToTable("guide_verification_submissions", Constants.Schema);

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(v => v.GuideId).HasMaxLength(450).IsRequired();
            builder.Property(v => v.Status).IsRequired();
            builder.Property(v => v.SubmittedAt).IsRequired();
            builder.Property(v => v.ReviewedAt);
            builder.Property(v => v.ReviewedByAdminId).HasMaxLength(450);
            builder.Property(v => v.AdminNotes).HasMaxLength(2000);
            builder.Property(v => v.RejectionReason).HasMaxLength(1000);
            builder.Property(v => v.CreatedAt).IsRequired();
            builder.Property(v => v.UpdatedAt).IsRequired();

            builder.HasOne(v => v.Guide)
                .WithMany()
                .HasForeignKey(v => v.GuideId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.Documents)
                .WithOne(d => d.Submission)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => v.GuideId);
            builder.HasIndex(v => v.Status);
            builder.HasIndex(v => v.SubmittedAt);
        }
    }

    public class GuideVerificationDocumentConfiguration : IEntityTypeConfiguration<GuideVerificationDocument>
    {
        public void Configure(EntityTypeBuilder<GuideVerificationDocument> builder)
        {
            builder.ToTable("guide_verification_documents", Constants.Schema);

            builder.HasKey(d => d.Id);
            builder.Property(d => d.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(d => d.SubmissionId).HasMaxLength(50).IsRequired();
            builder.Property(d => d.DocumentType).HasMaxLength(100).IsRequired();
            builder.Property(d => d.FileName).HasMaxLength(500).IsRequired();
            builder.Property(d => d.FileBase64).HasColumnType("nvarchar(max)");
            builder.Property(d => d.Status).IsRequired();
            builder.Property(d => d.UploadedAt).IsRequired();
            builder.Property(d => d.CreatedAt).IsRequired();

            builder.HasIndex(d => d.SubmissionId);
        }
    }
}
