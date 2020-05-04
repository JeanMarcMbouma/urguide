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
            builder.Property(x => x.ImageLink).IsRequired();

            builder.HasData(
                new Category
                {
                    Id = "d1442a22-adc5-4eab-a232-6ae1fe1ad4f5",
                    Archived = false,
                    Created = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    ImageLink = "images/sport.png",
                    LastUpdated = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    Name = "Sport"
                },
                new Category
                {
                    Id = "62cf86ff-755d-46fd-bf8d-ca08ba353451",
                    Archived = false,
                    Created = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    ImageLink = "images/nature.png",
                    LastUpdated = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    Name = "Nature"
                },
                new Category
                {
                    Id = "057e7c41-48a2-40af-83f7-86495daa66bb",
                    Archived = false,
                    Created = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    ImageLink = "images/child.png",
                    LastUpdated = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    Name = "Child"
                },
                new Category
                {
                    Id = "4dc654b1-c887-4000-8e53-309f2aad0e3d",
                    Archived = false,
                    Created = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    ImageLink = "images/historical.png",
                    LastUpdated = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    Name = "Historical"
                },
                new Category
                {
                    Id = "9d78cfc4-2299-445c-9c38-d6dd9d081f2b",
                    Archived = false,
                    Created = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    ImageLink = "images/amusement.png",
                    LastUpdated = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    Name = "Amusement"
                },
                new Category
                {
                    Id = "3f35dba7-d527-4c70-80cb-68d25ee2b332",
                    Archived = false,
                    Created = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    ImageLink = "images/extreme.png",
                    LastUpdated = new DateTime(2020, 5, 1, 12, 0, 0, 0, DateTimeKind.Unspecified),
                    Name = "Extreme"
                });
        }
    }
}
