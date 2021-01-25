using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UrGuide.Data.Entities.Tour;

namespace UrGuide.Data.Configurations
{
    class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("tour_booking", Constants.Schema);
            builder.HasKey(x => x.BookingId);
            builder.Property(x => x.BookingId).HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.EnablePushNotification);
            builder.HasOne(x => x.Author)
                .WithMany().HasForeignKey(x => x.AuthorId).IsRequired();
            builder.HasOne(x => x.Tour)
                .WithMany().HasForeignKey(x => x.TourId);
            builder.HasOne(x => x.Subscription)
                .WithMany();
            builder.HasOne(x => x.Region)
                .WithMany().HasForeignKey(x => x.RegionId).IsRequired();
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.UpdatedAt);
            var converter = new ValueConverter<BookingStatus, int>(
                v => (int)v,
                v => (BookingStatus)v);
            builder.Property(x => x.Status).HasConversion(converter);
            builder.Property(x => x.Amount).HasColumnType("float");
            builder.Property(x => x.When);
        }
    }
}
