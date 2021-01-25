using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    class BalanceConfiguration : IEntityTypeConfiguration<Balance>
    {
        public void Configure(EntityTypeBuilder<Balance> builder)
        {
            builder.ToTable("author_balance", Constants.Schema);
            builder.HasKey(x => x.BalanceId);
            builder.Property(x => x.BalanceId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Bonus).HasColumnType("float");
            builder.Property(x => x.Coins).HasColumnType("float");
            builder.Property(x => x.UpdatedAt);
            builder.HasOne(x => x.Region)
                .WithMany().IsRequired();
        }
    }
}
