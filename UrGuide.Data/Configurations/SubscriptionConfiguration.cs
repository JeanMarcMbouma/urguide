using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("subscriptions", Constants.Schema);
            builder.HasKey(x => x.SubscriptionId);
            builder.Property(x => x.SubscriptionId).HasDefaultValueSql(Constants.GuidFn);
            var converter = new ValueConverter<Membership, int>(
                v => (int)v,
                v => (Membership)v);
            builder.Property(x => x.Membership)
                .HasConversion(converter);
            builder.HasOne(x => x.Author)
                .WithMany().HasForeignKey(x => x.AuthorId).IsRequired();
            builder.HasOne(x => x.Region)
                .WithMany().HasForeignKey(x => x.RegionId).IsRequired();
            builder.Property(x => x.ActivatedOn);
            builder.OwnsOne(x => x.CreditCard, c => {
                c.Property(x => x.CardHolderName).HasMaxLength(Constants.TitleMaxLength).IsRequired();
                c.Property(x => x.CardNumber).HasMaxLength(16);
                c.Ignore(x => x.Expired);
                c.Property(x => x.ExpiryMonth);
                c.Property(x => x.ExpiryYear);
            });
        }
    }
}
