using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
    {
        public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
        {
            builder.ToTable("loyalty_accounts", Constants.Schema);
            builder.HasKey(x => x.LoyaltyAccountId);
            builder.Property(x => x.LoyaltyAccountId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Points).HasDefaultValue(0);
            builder.Property(x => x.Tier);
            builder.Property(x => x.DiscountPercentage).HasDefaultValue(0);
            builder.Property(x => x.TotalToursCompleted).HasDefaultValue(0);
            builder.Property(x => x.TotalSpent).HasPrecision(18, 2).HasDefaultValue(0m);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.HasIndex(x => x.UserId).IsUnique();
            builder.HasMany(x => x.Transactions)
                .WithOne(x => x.LoyaltyAccount)
                .HasForeignKey(x => x.LoyaltyAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
