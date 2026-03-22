using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Media;

namespace UrGuide.Data.Configurations
{
    internal class ProcessedImageConfiguration : IEntityTypeConfiguration<ProcessedImage>
    {
        public void Configure(EntityTypeBuilder<ProcessedImage> builder)
        {
            builder.ToTable("processed_images", Constants.Schema);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasMaxLength(128)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(x => x.OriginalImageId)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.OriginalUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength)
                .IsRequired();

            builder.Property(x => x.ThumbnailUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(x => x.MediumUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(x => x.LargeUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(x => x.WebPUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(x => x.Format)
                .HasMaxLength(20);

            builder.Property(x => x.OriginalSize)
                .IsRequired();

            builder.Property(x => x.CompressedSize)
                .IsRequired();

            builder.Property(x => x.Width)
                .IsRequired();

            builder.Property(x => x.Height)
                .IsRequired();

            builder.Property(x => x.CdnUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);

            builder.Property(x => x.IsWatermarked)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(x => x.ExifDataJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ProcessedAt);

            builder.HasIndex(x => x.OriginalImageId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
