using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Configurations
{
    class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("countries", Constants.Schema);
            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name).HasColumnName("CountryId")
                .HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Code).IsRequired()
                .HasMaxLength(10);
            builder.Property(x => x.DialCode).IsRequired()
                .HasMaxLength(7);
        }
    }
}
