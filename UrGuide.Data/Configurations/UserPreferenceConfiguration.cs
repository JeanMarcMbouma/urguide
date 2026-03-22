using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Recommendations;

namespace UrGuide.Data.Configurations
{
    public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
    {
        public void Configure(EntityTypeBuilder<UserPreference> builder)
        {
            builder.ToTable("user_preferences", Constants.Schema);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(50)
                .HasDefaultValueSql(Constants.GuidFn)
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(e => e.PreferenceType)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.PreferenceValue)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.Weight)
                .HasPrecision(5, 2)
                .HasDefaultValue(1.0m)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => new { e.UserId, e.PreferenceType });
        }
    }
}
