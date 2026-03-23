using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Recommendations;

namespace UrGuide.Data.Configurations
{
    public class RecommendationLogConfiguration : IEntityTypeConfiguration<RecommendationLog>
    {
        public void Configure(EntityTypeBuilder<RecommendationLog> builder)
        {
            builder.ToTable("recommendation_logs", Constants.Schema);

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

            builder.Property(e => e.Score)
                .HasPrecision(10, 4)
                .IsRequired();

            builder.Property(e => e.Algorithm)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.WasClicked)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(e => e.WasBooked)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.TourId);
            builder.HasIndex(e => e.Algorithm);
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}
