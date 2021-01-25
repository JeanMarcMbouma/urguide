using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Configurations
{
    class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("currencies", Constants.Schema);
            builder.HasKey(x => x.Name);
            builder.Property(x => x.Name).HasColumnName("CurrencyId")
                .HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Code).IsRequired()
                .HasMaxLength(10);
            builder.Property(x => x.DecimalDigits);
            builder.Property(x => x.NamePlural).HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Rounding);
            builder.Property(x => x.Symbol);
            builder.Property(x => x.SymbolNative);
        }
    }
}
