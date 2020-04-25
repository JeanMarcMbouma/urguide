using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Posts;

namespace UrGuide.Data.Configurations
{
    class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Post_Categories", Constants.Schema);
            builder.HasKey(x => x.Id).HasName("PK_Post_Categories");
            builder.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn)
                .HasColumnName("CategoryId");
            builder.Property(x => x.Created).IsRequired();
            builder.Property(x => x.Archived);
            builder.Property(x => x.LastUpdated);
            builder.Property(x => x.Name).HasColumnName("CategoryName")
                .IsRequired().HasMaxLength(200);

            builder.OwnsOne(x => x.Image);
            builder.OwnsMany(x => x.Attributes, a =>
            {
                a.ToTable("Post_Categories_Attributes", Constants.Schema);
                a.WithOwner().HasForeignKey("CategoryId");
                a.Property(x => x.Name).IsRequired().HasMaxLength(200);
                a.Property(x => x.Value).IsRequired();
                a.HasKey("Id");
            });
        }
    }
}
