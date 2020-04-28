using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", Constants.Schema);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("UserId").IsRequired();
            builder.Property(x => x.LastActivityDate);
            builder.OwnsOne(x => x.ProfileImage, p => {
                p.ToTable("User_Images", Constants.Schema);
                p.WithOwner().HasForeignKey("UserId");
                p.HasKey(x => x.Id).HasName("PK_User_Images");
                p.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                p.Property(x => x.ImageBase64).IsRequired();
            });
            builder.OwnsMany(x => x.Attributes, a =>
            {
                a.ToTable("User_Attributes", Constants.Schema);
                a.WithOwner().HasForeignKey("UserId");
                a.Property(x => x.Name).IsRequired().HasMaxLength(200);
                a.Property(x => x.Value).IsRequired();
                a.HasKey("Id");
            });
        }
    }
}
