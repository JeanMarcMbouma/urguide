using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class LotteryEntryConfiguration : IEntityTypeConfiguration<LotteryEntry>
    {
        public void Configure(EntityTypeBuilder<LotteryEntry> builder)
        {
            builder.ToTable("lottery_entries", Constants.Schema);
            builder.HasKey(x => x.LotteryEntryId);
            builder.Property(x => x.LotteryEntryId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.LotteryDrawId).IsRequired();
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.IsWinner).HasDefaultValue(false);
            builder.Property(x => x.EnteredAt);
            builder.HasIndex(x => new { x.LotteryDrawId, x.UserId }).IsUnique();
        }
    }
}
