using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Premium;

namespace UrGuide.Data.Configurations
{
    class VisibilityBoostConfiguration : IEntityTypeConfiguration<VisibilityBoost>
    {
        public void Configure(EntityTypeBuilder<VisibilityBoost> builder)
        {
            builder.ToTable("visibility_boosts", Constants.Schema);
            builder.HasKey(x => x.VisibilityBoostId);
            builder.Property(x => x.VisibilityBoostId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.GuideId).HasMaxLength(450);
            builder.Property(x => x.TourId).HasMaxLength(450);
            builder.Property(x => x.BoostType);
            builder.Property(x => x.Status);
            builder.Property(x => x.BoostMultiplier).HasDefaultValue(1);
            builder.Property(x => x.StartDate);
            builder.Property(x => x.EndDate);
            builder.Property(x => x.Cost).HasPrecision(18, 2);
            builder.Property(x => x.CreatedAt);
            builder.HasIndex(x => x.GuideId);
            builder.HasIndex(x => x.Status);
        }
    }
}
