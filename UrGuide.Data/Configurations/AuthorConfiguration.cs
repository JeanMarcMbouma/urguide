using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("authors", Constants.Schema);
            builder.HasKey(x => x.AuthorId);
            builder.Property(x => x.AuthorId).IsRequired();

            builder.Property(x => x.Rating);
            builder.HasOne(x => x.Balance)
                .WithMany().HasForeignKey(x => x.BalanceId);

            builder.HasOne(x => x.Subscription)
                .WithMany().HasForeignKey(x => x.SubscriptionId);

            builder.OwnsOne(x => x.ProfileInfo, p => {
                p.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
                p.Property(x => x.ImageUrl)
                .HasMaxLength(Constants.ImageUrlMaxLength);
                p.Property(x => x.PhoneNumber).HasMaxLength(20);
                p.Property(x => x.CreatedAt).IsRequired();
                p.Property(x => x.UpdatedAt);
            });
            builder.OwnsOne(x => x.Activity);
        }
    }
}
