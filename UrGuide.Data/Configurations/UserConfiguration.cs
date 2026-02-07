using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
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
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(200).HasDefaultValue(Constants.NA);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(200).HasDefaultValue(Constants.NA);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(255).HasDefaultValue(Constants.NA);
            builder.Property(x => x.UserName).HasMaxLength(255);
            builder.Property(x => x.StripeCustomerId).HasMaxLength(100);
            builder.Ignore(x => x.FullName);
            builder.Property(x => x.Location);

            builder.OwnsOne(x => x.ProfileImage, p => {
                p.ToTable("User_Images", Constants.Schema);
                p.WithOwner().HasForeignKey("UserId");
                p.HasKey(x => x.Id).HasName("PK_User_Images");
                p.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                p.Property(x => x.ImageUrl).HasColumnName("ImageBase64").IsRequired();
            });
            builder.OwnsMany(x => x.Attributes, a =>
            {
                a.ToTable("User_Attributes", Constants.Schema);
                a.WithOwner().HasForeignKey("UserId");
                a.Property(x => x.Name).IsRequired().HasMaxLength(200);
                a.Property(x => x.Value).IsRequired();
                a.HasKey("Id");
            });

            builder.OwnsMany(x => x.Notifications, a =>
            {
                a.ToTable("User_Notifications", Constants.Schema);
                a.WithOwner().HasForeignKey("UserId");
                a.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                a.Property(x => x.Content).IsRequired().HasMaxLength(500);
                a.Property(x => x.Created).HasColumnType("datetime2");
                a.Property(x => x.ReferenceLink).HasMaxLength(1000);
                a.Property(x => x.Read);
                a.Property(x => x.IsSystem);
                a.HasOne(x => x.Sender).WithMany().HasForeignKey("FK_User_Notification_Users");
                a.HasKey(x => x.Id);
            });

            builder.OwnsMany(x => x.Feedback, b =>
            {
                b.ToTable("User_Feedback", Constants.Schema);
                b.WithOwner().HasForeignKey("UserId");
                b.HasKey(x => x.Id).HasName("PK_User_Feedback");
                b.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                b.Property(x => x.Text).IsRequired().HasMaxLength(2000);
                b.Property(x => x.Created).IsRequired();
                b.Property(x => x.Rating).IsRequired();
                b.HasOne(x => x.Author).WithMany().HasForeignKey("FK_User_Feedback_Users");
            });

            string systemUserId = "00000000-0000-0000-0000-000000000000";
            var system = new User
            {
                Id = systemUserId,
                LastActivityDate = new DateTime(2020, 1, 1, 12, 0, 0)
            };
            builder.HasData(system);
        }
    }
}
