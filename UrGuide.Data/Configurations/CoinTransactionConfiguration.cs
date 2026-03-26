using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Financial;

namespace UrGuide.Data.Configurations
{
    class CoinTransactionConfiguration : IEntityTypeConfiguration<CoinTransaction>
    {
        public void Configure(EntityTypeBuilder<CoinTransaction> builder)
        {
            builder.ToTable("coin_transactions", Constants.Schema);
            builder.HasKey(x => x.CoinTransactionId);
            builder.Property(x => x.CoinTransactionId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.CoinWalletId).IsRequired();
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.TransactionType);
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.ReferenceId).HasMaxLength(450);
            builder.Property(x => x.CreatedAt);
            builder.HasIndex(x => x.CoinWalletId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
