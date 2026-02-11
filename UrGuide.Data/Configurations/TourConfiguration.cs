using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            builder.ToTable("tours", Constants.Schema);
            builder.HasKey(x => x.TourId);
            builder.Property(x => x.TourId).HasDefaultValueSql(Constants.GuidFn);
            builder.HasOne(x => x.Author).WithMany()
                .HasForeignKey(x => x.AuthorId).IsRequired();
            builder.OwnsOne(x => x.Stats);
            builder.OwnsOne(x => x.Schedule, s => {
                s.Ignore(x => x.Active);
            });

            builder.Property(x => x.Title).IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description).IsRequired()
                .HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.Tags).IsRequired();

            builder.HasOne(x => x.Region)
                .WithMany().HasForeignKey(x => x.RegionId).IsRequired();

            builder.HasMany(x => x.Reviews)
                .WithOne();
            builder.OwnsMany(x => x.MapPins, m => {
                m.ToTable("tours_map_pins", Constants.Schema);
                m.WithOwner().HasForeignKey("TourId");
                m.HasKey(x => x.MapPinId);
                m.Property(x => x.MapPinId).HasDefaultValueSql(Constants.GuidFn);
                m.Property(x => x.Title).IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
                m.Property(x => x.Description).IsRequired()
                .HasMaxLength(Constants.DescriptionMaxLength);
                m.Property(x => x.Latitude).HasColumnType("float");
                m.Property(x => x.Longitude).HasColumnType("float");
                m.Property(x => x.ImageUrl).IsRequired()
                .HasMaxLength(Constants.ImageUrlMaxLength);
            });
            builder.HasMany(x => x.Bookings)
                .WithOne();
            builder.OwnsMany(x => x.Reactions, r => {
                r.ToTable("tour_reactions", Constants.Schema);
                r.WithOwner().HasForeignKey("TourId");
                r.HasKey(x => x.ReactionId);
                r.Property(x => x.ReactionId).HasDefaultValueSql(Constants.GuidFn);
                r.Property(x => x.AuthorId).IsRequired();
            });
        }
    }
}
