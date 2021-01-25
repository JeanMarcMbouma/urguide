using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Tour;
using UrGuide.Data.Entities.Users;

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
            builder.HasOne(x => x.Author)
                .WithMany().IsRequired();
        }
    }
}
