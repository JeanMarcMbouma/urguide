using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Messages;

namespace UrGuide.Data.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<ConversationEntity>
    {
        public void Configure(EntityTypeBuilder<ConversationEntity> builder)
        {
            builder.ToTable("conversations", Constants.Schema);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(c => c.Participant1Id).HasMaxLength(450).IsRequired();
            builder.Property(c => c.Participant1Name).HasMaxLength(256);
            builder.Property(c => c.Participant2Id).HasMaxLength(450).IsRequired();
            builder.Property(c => c.Participant2Name).HasMaxLength(256);
            builder.Property(c => c.LastMessage).HasMaxLength(2000);
            builder.Property(c => c.LastMessageAt).IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();

            builder.HasIndex(c => c.Participant1Id);
            builder.HasIndex(c => c.Participant2Id);
        }
    }

    public class MessageEntityConfiguration : IEntityTypeConfiguration<MessageEntity>
    {
        public void Configure(EntityTypeBuilder<MessageEntity> builder)
        {
            builder.ToTable("messages", Constants.Schema);

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).HasMaxLength(50).IsRequired().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(m => m.ConversationId).HasMaxLength(50).IsRequired();
            builder.Property(m => m.SenderId).HasMaxLength(450).IsRequired();
            builder.Property(m => m.SenderName).HasMaxLength(256);
            builder.Property(m => m.Content).HasMaxLength(2000).IsRequired();
            builder.Property(m => m.SentAt).IsRequired();
            builder.Property(m => m.IsRead).IsRequired().HasDefaultValue(false);

            builder.HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(m => m.ConversationId);
            builder.HasIndex(m => m.SenderId);
        }
    }
}
