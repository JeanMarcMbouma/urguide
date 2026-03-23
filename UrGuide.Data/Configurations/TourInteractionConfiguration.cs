using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Recommendations;

namespace UrGuide.Data.Configurations
{
    public class TourInteractionConfiguration : IEntityTypeConfiguration<TourInteraction>
    {
        public void Configure(EntityTypeBuilder<TourInteraction> builder)
        {
            builder.ToTable("tour_interactions", Constants.Schema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.TourId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Type)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.TourId);
            builder.HasIndex(e => new { e.UserId, e.TourId, e.Type });
        }
    }
}
