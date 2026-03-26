using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
    {
        public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
        {
            builder.ToTable("loyalty_transactions", Constants.Schema);
            builder.HasKey(x => x.LoyaltyTransactionId);
            builder.Property(x => x.LoyaltyTransactionId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.LoyaltyAccountId).IsRequired();
            builder.Property(x => x.Points);
            builder.Property(x => x.TransactionType);
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.ReferenceId).HasMaxLength(450);
            builder.Property(x => x.CreatedAt);
            builder.HasIndex(x => x.LoyaltyAccountId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
