using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class TourRequestConfiguration : IEntityTypeConfiguration<TourRequest>
    {
        public void Configure(EntityTypeBuilder<TourRequest> builder)
        {
            builder.ToTable("tour_requests", Constants.Schema);
            builder.HasKey(x => x.TourRequestId);
            builder.Property(x => x.TourRequestId).HasDefaultValueSql(Constants.GuidFn);

            builder.Property(x => x.Title).IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description).IsRequired()
                .HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.Tags).HasMaxLength(500);

            builder.Property(x => x.MaxParticipants).IsRequired();
            builder.Property(x => x.MaxBudget).HasColumnType("decimal(18,2)");
            builder.Property(x => x.PreferredDate).IsRequired();
            builder.Property(x => x.Status).IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.UpdatedAt).IsRequired();

            // Relationships
            builder.HasOne(x => x.Requester)
                .WithMany()
                .HasForeignKey(x => x.RequesterId)
                .IsRequired();

            builder.HasOne(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(x => x.RegionId);
            builder.HasIndex(x => x.RequesterId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}