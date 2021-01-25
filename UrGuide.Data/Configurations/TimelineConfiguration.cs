using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Configurations
{
    class TimelineConfiguration : IEntityTypeConfiguration<Timeline>
    {
        public void Configure(EntityTypeBuilder<Timeline> builder)
        {
            builder.ToTable("region_timelines", Constants.Schema);
            builder.HasKey(x => x.TimelineId);
            builder.Property(x => x.TimelineId).HasDefaultValueSql(Constants.GuidFn);

            builder.HasMany(x => x.Items)
                .WithOne();
            builder.HasMany(x => x.Campains)
                .WithOne();
        }
    }
}
