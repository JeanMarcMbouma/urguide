using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("reviews", Constants.Schema);
            builder.HasKey(x => x.ReviewId);
            builder.Property(x => x.ReviewId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Text).HasMaxLength(Constants.DescriptionMaxLength)
                .IsRequired();
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            builder.Property(x => x.Rating);
            builder.Property(x => x.ModerationStatus)
                .HasDefaultValue(ReviewModerationStatus.Pending);
            builder.Property(x => x.IsSpam)
                .HasDefaultValue(false);
            builder.Property(x => x.SpamScore)
                .HasPrecision(5, 2)
                .HasDefaultValue(0m);
            builder.HasOne(x => x.Author)
                .WithMany().IsRequired();
            builder.HasMany(x => x.Flags)
                .WithOne(x => x.Review)
                .HasForeignKey(x => x.ReviewId);
            builder.HasMany(x => x.ModerationActions)
                .WithOne(x => x.Review)
                .HasForeignKey(x => x.ReviewId);
        }
    }
}
