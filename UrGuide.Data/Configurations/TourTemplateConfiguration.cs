using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class TourTemplateConfiguration : IEntityTypeConfiguration<TourTemplate>
    {
        public void Configure(EntityTypeBuilder<TourTemplate> builder)
        {
            builder.ToTable("tour_templates", Constants.Schema);
            builder.HasKey(x => x.TemplateId);
            builder.Property(x => x.TemplateId).HasDefaultValueSql(Constants.GuidFn);

            builder.Property(x => x.GuideId).IsRequired();

            builder.Property(x => x.Name).IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Description)
                .HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.Category)
                .HasMaxLength(Constants.TitleMaxLength);

            builder.Property(x => x.BasePrice)
                .HasPrecision(18, 2);
            builder.Property(x => x.CurrencyCode)
                .HasMaxLength(10);

            builder.Property(x => x.DefaultDurationMinutes);
            builder.Property(x => x.DefaultMaxParticipants);
            builder.Property(x => x.DefaultMeetingPoint)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(x => x.ItineraryJson);
            builder.Property(x => x.IncludedItemsJson);
            builder.Property(x => x.ExcludedItemsJson);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);
            builder.Property(x => x.UsageCount)
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
        }
    }
}
