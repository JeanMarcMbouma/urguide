using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    internal class DataExportRequestConfiguration : IEntityTypeConfiguration<DataExportRequest>
    {
        public void Configure(EntityTypeBuilder<DataExportRequest> builder)
        {
            builder.ToTable("DataExportRequests", Constants.Schema);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(128);
            
            builder.Property(x => x.UserId).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.Format).IsRequired();
            builder.Property(x => x.RequestedAt).IsRequired();
            builder.Property(x => x.ExpiresAt).IsRequired();
            builder.Property(x => x.DownloadToken).HasMaxLength(256);
            builder.Property(x => x.FilePath).HasMaxLength(500);
            builder.Property(x => x.FileSizeBytes);
            
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.DownloadToken)
                   .IsUnique()
                   .HasFilter("[DownloadToken] IS NOT NULL");
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}
