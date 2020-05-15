using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Shared;

namespace UrGuide.Data.Configurations
{
    class ImageCatalogConfiguration : IEntityTypeConfiguration<ImageCatalog>
    {
        public void Configure(EntityTypeBuilder<ImageCatalog> builder)
        {
            builder.ToTable("Image_Catalogs", Constants.Schema);
            builder.HasKey(x => x.Id).HasName("PK_Image_Catalogs");
            builder.Property(x => x.Id).HasColumnName("Image_CatalogId").HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Created).IsRequired();
            builder.Property(x => x.LastUpdated);
            builder.HasOne(x => x.User)
                .WithMany().HasForeignKey("UserId");
            builder.Property(x => x.Location);
            builder.OwnsMany(x => x.Images, i => {
                i.ToTable("Image_Catalog_Files", Constants.Schema);
                i.WithOwner().HasForeignKey("Image_CatalogId");
                i.HasKey(x => x.Id).HasName("PK_Image_Catalog_Files");
                i.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                i.Property(x => x.ImageUrl).HasColumnName("FileBase64").IsRequired();
                i.Property(x => x.MimeType).IsRequired();
                i.OwnsMany(x => x.Attributes, a =>
                {
                    a.ToTable("File_Attributes", Constants.Schema);
                    a.WithOwner().HasForeignKey("FileId");
                    a.Property(x => x.Name).IsRequired().HasMaxLength(200);
                    a.Property(x => x.Value).IsRequired();
                    a.HasKey("Id");
                });
            });
        }
    }
}
