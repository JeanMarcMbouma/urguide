using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class CampainConfiguration : IEntityTypeConfiguration<Campain>
    {
        public void Configure(EntityTypeBuilder<Campain> builder)
        {
            builder.ToTable("campains", Constants.Schema);
            builder.HasKey(x => x.CampainId);
            builder.Property(x => x.CampainId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.ActiveFrom);
            builder.Property(x => x.ActiveUntil);
            builder.Property(x => x.DescriptionSEO).IsRequired()
                .HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.DiscountPercentage);
            builder.Property(x => x.ImageUrl).HasMaxLength(Constants.ImageUrlMaxLength);
            var converter = new ValueConverter<Membership, int>(
                v => (int)v,
                v => (Membership)v);
            builder.Property(x => x.Membership).HasConversion(converter);
            builder.HasOne(x => x.Region)
                .WithMany().HasForeignKey(x => x.RegionId).IsRequired();
        }
    }
}
