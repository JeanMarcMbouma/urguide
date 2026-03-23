using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Messages;

namespace UrGuide.Data.Configurations
{
    public class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
    {
        public void Configure(EntityTypeBuilder<FileAttachment> builder)
        {
            builder.ToTable("file_attachments", Constants.Schema);

            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(f => f.MessageId).HasMaxLength(50).IsRequired();
            builder.Property(f => f.FileName).HasMaxLength(500).IsRequired();
            builder.Property(f => f.FileUrl).HasMaxLength(2000).IsRequired();
            builder.Property(f => f.FileSize).IsRequired();
            builder.Property(f => f.ContentType).HasMaxLength(200).IsRequired();
            builder.Property(f => f.UploadedAt).IsRequired();

            builder.HasOne(f => f.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(f => f.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(f => f.MessageId);
        }
    }
}
