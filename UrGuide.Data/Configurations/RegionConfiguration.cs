using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Configurations
{
    class RegionConfiguration : IEntityTypeConfiguration<Region>
    {
        public void Configure(EntityTypeBuilder<Region> builder)
        {
            builder.ToTable("regions", Constants.Schema);
            builder.HasKey(x => x.RegionId);
            builder.Property(x => x.RegionId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
            builder.OwnsOne(x => x.Stats);
            builder.OwnsOne(x => x.Flags);
            builder.HasOne(x => x.Country)
                .WithMany(c => c.Regions).HasForeignKey(x => x.CountryId).IsRequired();
            builder.HasOne(x => x.PaymentMethod)
                .WithMany().HasForeignKey(x => x.PaymentMethodId);
            builder.HasOne(x => x.Timeline)
                .WithMany().HasForeignKey(x => x.TimelineId);
            builder.HasOne(x => x.Currency)
                .WithMany().HasForeignKey(x => x.CurrencyId);
        }
    }
}
