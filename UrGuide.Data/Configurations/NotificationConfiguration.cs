using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Messages;

namespace UrGuide.Data.Configurations
{
    class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", Constants.Schema);
            builder.HasKey(x => x.Id).HasName("PK_Messages");
            builder.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn)
                .HasColumnName("MessageId");
            builder.Property(x => x.Created).IsRequired();
            builder.Property(x => x.HasError).HasDefaultValue(false);
            builder.Property(x => x.Sent).HasDefaultValue(false);
            builder.Property(x => x.To)
                .IsRequired().HasMaxLength(200);

            builder.Property(x => x.Subject)
                .IsRequired().HasMaxLength(200);

            builder.Property(x => x.Content)
                .IsRequired().HasMaxLength(1000);

            builder.OwnsMany(x => x.Links, l =>
            {
                l.ToTable("Message_Links", Constants.Schema);
                l.WithOwner().HasForeignKey("MessageId");
                l.Property(x => x.Id).IsRequired().HasDefaultValueSql(Constants.GuidFn);
                l.Property(x => x.Token).IsRequired().HasMaxLength(100);
                l.Property(x => x.Url).IsRequired().HasMaxLength(2000);
                l.HasKey(x => x.Id);
            });
        }
    }
}
