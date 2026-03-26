using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Financial;

namespace UrGuide.Data.Configurations
{
    class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
    {
        public void Configure(EntityTypeBuilder<WithdrawalRequest> builder)
        {
            builder.ToTable("withdrawal_requests", Constants.Schema);
            builder.HasKey(x => x.WithdrawalRequestId);
            builder.Property(x => x.WithdrawalRequestId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.CurrencyCode).IsRequired().HasMaxLength(3);
            builder.Property(x => x.BankName).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.AccountNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.RoutingNumber).HasMaxLength(50);
            builder.Property(x => x.AccountHolderName).IsRequired().HasMaxLength(Constants.TitleMaxLength);
            builder.Property(x => x.Status);
            builder.Property(x => x.TransactionReference).HasMaxLength(450);
            builder.Property(x => x.FailureReason).HasMaxLength(Constants.DescriptionMaxLength);
            builder.Property(x => x.RequestedAt);
            builder.Property(x => x.ProcessedAt);
            builder.Property(x => x.CompletedAt);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Status);
        }
    }
}
