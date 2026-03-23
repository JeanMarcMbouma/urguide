using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class ReviewModerationActionConfiguration : IEntityTypeConfiguration<ReviewModerationAction>
    {
        public void Configure(EntityTypeBuilder<ReviewModerationAction> builder)
        {
            builder.ToTable("review_moderation_actions", Constants.Schema);
            builder.HasKey(x => x.ActionId);
            builder.Property(x => x.ActionId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.ReviewId).IsRequired();
            builder.Property(x => x.PerformedBy).IsRequired();
            builder.Property(x => x.ActionType);
            builder.Property(x => x.Reason).HasMaxLength(Constants.DescriptionMaxLength)
                .IsRequired();
            builder.Property(x => x.PreviousContent).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.CreatedAt);

            builder.HasOne(x => x.Review)
                .WithMany(x => x.ModerationActions)
                .HasForeignKey(x => x.ReviewId);
            builder.HasOne(x => x.PerformedByUser)
                .WithMany()
                .HasForeignKey(x => x.PerformedBy);
        }
    }
}
