using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class ReviewFlagConfiguration : IEntityTypeConfiguration<ReviewFlag>
    {
        public void Configure(EntityTypeBuilder<ReviewFlag> builder)
        {
            builder.ToTable("review_flags", Constants.Schema);
            builder.HasKey(x => x.ReviewFlagId);
            builder.Property(x => x.ReviewFlagId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.ReviewId).IsRequired();
            builder.Property(x => x.FlaggedBy).IsRequired();
            builder.Property(x => x.Reason).HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();
            builder.Property(x => x.Description).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.Status)
                .HasDefaultValue(ReviewFlagStatus.Pending);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.ResolvedAt);
            builder.Property(x => x.ResolvedBy);

            builder.HasOne(x => x.Review)
                .WithMany(x => x.Flags)
                .HasForeignKey(x => x.ReviewId);
            builder.HasOne(x => x.FlaggedByUser)
                .WithMany()
                .HasForeignKey(x => x.FlaggedBy);
        }
    }
}
