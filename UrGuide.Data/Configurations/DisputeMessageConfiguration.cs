using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Disputes;

namespace UrGuide.Data.Configurations
{
    public class DisputeMessageConfiguration : IEntityTypeConfiguration<DisputeMessage>
    {
        public void Configure(EntityTypeBuilder<DisputeMessage> builder)
        {
            builder.ToTable("dispute_messages", Constants.Schema);

            builder.HasKey(m => m.MessageId);

            builder.Property(m => m.MessageId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.DisputeId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.SenderId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(m => m.SenderName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Content)
                .HasMaxLength(Constants.DescriptionMaxLength)
                .IsRequired();

            builder.Property(m => m.IsAdminMessage)
                .IsRequired();

            builder.Property(m => m.SentAt)
                .IsRequired();

            builder.HasIndex(m => m.DisputeId);
            builder.HasIndex(m => m.SenderId);
            builder.HasIndex(m => m.SentAt);
        }
    }
}
