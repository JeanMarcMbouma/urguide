using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    class AccountFreezeRecordConfiguration : IEntityTypeConfiguration<AccountFreezeRecord>
    {
        public void Configure(EntityTypeBuilder<AccountFreezeRecord> builder)
        {
            builder.ToTable("Account_Freeze_Records", Constants.Schema);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.Reason).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.FrozenByAdminId).IsRequired().HasMaxLength(450);
            builder.Property(x => x.FrozenAt).IsRequired().HasColumnType("datetime2");
            builder.Property(x => x.ExpiresAt).HasColumnType("datetime2");
            builder.Property(x => x.UnfrozenAt).HasColumnType("datetime2");
            builder.Property(x => x.UnfrozenByAdminId).HasMaxLength(450);
            builder.Property(x => x.UnfreezeReason).HasMaxLength(2000);
            var statusConverter = new ValueConverter<AccountFreezeStatus, int>(
                v => (int)v,
                v => (AccountFreezeStatus)v);
            builder.Property(x => x.Status).HasConversion(statusConverter).IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.Status);
        }
    }
}
