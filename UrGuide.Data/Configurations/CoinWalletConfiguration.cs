using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Financial;

namespace UrGuide.Data.Configurations
{
    class CoinWalletConfiguration : IEntityTypeConfiguration<CoinWallet>
    {
        public void Configure(EntityTypeBuilder<CoinWallet> builder)
        {
            builder.ToTable("coin_wallets", Constants.Schema);
            builder.HasKey(x => x.CoinWalletId);
            builder.Property(x => x.CoinWalletId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Balance).HasPrecision(18, 2).HasDefaultValue(0m);
            builder.Property(x => x.TotalEarned).HasPrecision(18, 2).HasDefaultValue(0m);
            builder.Property(x => x.TotalSpent).HasPrecision(18, 2).HasDefaultValue(0m);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasIndex(x => x.UserId).IsUnique();
            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.CoinWallet)
                .HasForeignKey(x => x.CoinWalletId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
