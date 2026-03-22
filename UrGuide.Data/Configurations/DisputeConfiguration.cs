using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Disputes;

namespace UrGuide.Data.Configurations
{
    public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
    {
        public void Configure(EntityTypeBuilder<Dispute> builder)
        {
            builder.ToTable("disputes", Constants.Schema);

            builder.HasKey(d => d.DisputeId);

            builder.Property(d => d.DisputeId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.BookingId)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(d => d.FiledBy)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(d => d.AgainstUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(d => d.Title)
                .HasMaxLength(Constants.TitleMaxLength)
                .IsRequired();

            builder.Property(d => d.Description)
                .HasMaxLength(Constants.DescriptionMaxLength)
                .IsRequired();

            builder.Property(d => d.Category)
                .IsRequired();

            builder.Property(d => d.Status)
                .IsRequired();

            builder.Property(d => d.Priority)
                .IsRequired();

            builder.Property(d => d.AssignedTo)
                .HasMaxLength(450);

            builder.Property(d => d.Resolution)
                .HasMaxLength(Constants.DescriptionMaxLength);

            builder.Property(d => d.RefundAmount)
                .HasPrecision(18, 2);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt)
                .IsRequired();

            builder.Property(d => d.ResolvedAt);

            builder.HasMany(d => d.Evidence)
                .WithOne(e => e.Dispute)
                .HasForeignKey(e => e.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(d => d.Messages)
                .WithOne(m => m.Dispute)
                .HasForeignKey(m => m.DisputeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(d => d.BookingId);
            builder.HasIndex(d => d.FiledBy);
            builder.HasIndex(d => d.AgainstUserId);
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.Priority);
            builder.HasIndex(d => d.CreatedAt);
        }
    }
}
