using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Regions;

namespace UrGuide.Data.Configurations
{
    class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("payment_methods", Constants.Schema);
            builder.HasKey(x => x.PaymentMethodId);
            builder.Property(x => x.PaymentMethodId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Name).IsRequired()
                .HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.ApiKey);
            builder.Property(x => x.Secret);
            builder.Property(x => x.Secret2);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
        }
    }
}
