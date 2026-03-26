using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Premium;

namespace UrGuide.Data.Configurations
{
    class AdvertisementConfiguration : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder.ToTable("advertisements", Constants.Schema);
            builder.HasKey(x => x.AdvertisementId);
            builder.Property(x => x.AdvertisementId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.AdvertiserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Content).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.ImageUrl).HasMaxLength(Constants.ImageUrlMaxLength);
            builder.Property(x => x.TargetUrl).HasMaxLength(Constants.ImageUrlMaxLength);
            builder.Property(x => x.TargetAudience);
            builder.Property(x => x.TargetRegionId).HasMaxLength(450);
            builder.Property(x => x.Status);
            builder.Property(x => x.Budget).HasPrecision(18, 2);
            builder.Property(x => x.SpentAmount).HasPrecision(18, 2).HasDefaultValue(0m);
            builder.Property(x => x.Impressions).HasDefaultValue(0);
            builder.Property(x => x.Clicks).HasDefaultValue(0);
            builder.Property(x => x.StartDate);
            builder.Property(x => x.EndDate);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasIndex(x => x.AdvertiserId);
            builder.HasIndex(x => x.Status);
        }
    }
}
