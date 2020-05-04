using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
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
            builder.Property(x => x.Image).IsRequired();

            builder.HasData(new Category
            {
                Created = new System.DateTime(2020, 05, 1, 12, 0,0),
                Name = "Sport",
                LastUpdated = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Image = "images/sport.png",
                Id = Guid.NewGuid().ToString("D")
            }, new Category
            {
                Created = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Name = "Nature",
                LastUpdated = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Image = "images/nature.png",
                Id = Guid.NewGuid().ToString("D")
            }, new Category
            {
                Created = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Name = "Child",
                LastUpdated = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Image = "images/child.png",
                Id = Guid.NewGuid().ToString("D")
            }, new Category
            {
                Created = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Name = "Historical",
                LastUpdated = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Image = "images/historical.png",
                Id = Guid.NewGuid().ToString("D")
            }, new Category
            {
                Created = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Name = "Amusement",
                LastUpdated = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Image = "images/amusement.png",
                Id = Guid.NewGuid().ToString("D")
            }, new Category
            {
                Created = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Name = "Extreme",
                LastUpdated = new System.DateTime(2020, 05, 1, 12, 0, 0),
                Image = "images/extreme.png",
                Id = Guid.NewGuid().ToString("D")
            });
        }
    }
}
