using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Gamification;

namespace UrGuide.Data.Configurations
{
    class LotteryDrawConfiguration : IEntityTypeConfiguration<LotteryDraw>
    {
        public void Configure(EntityTypeBuilder<LotteryDraw> builder)
        {
            builder.ToTable("lottery_draws", Constants.Schema);
            builder.HasKey(x => x.LotteryDrawId);
            builder.Property(x => x.LotteryDrawId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.TourId).HasMaxLength(450);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.MaxEntries);
            builder.Property(x => x.WinnerCount).HasDefaultValue(1);
            builder.Property(x => x.Status);
            builder.Property(x => x.EntryDeadline);
            builder.Property(x => x.DrawDate);
            builder.Property(x => x.CreatedAt);
            builder.HasMany(x => x.Entries)
                .WithOne(x => x.LotteryDraw)
                .HasForeignKey(x => x.LotteryDrawId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => x.Status);
        }
    }
}
