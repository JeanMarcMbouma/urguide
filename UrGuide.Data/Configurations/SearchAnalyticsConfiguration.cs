using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrGuide.Data.Entities.Search;

namespace UrGuide.Data.Configurations
{
    internal class SearchAnalyticsConfiguration : IEntityTypeConfiguration<SearchAnalytics>
    {
        public void Configure(EntityTypeBuilder<SearchAnalytics> builder)
        {
            builder.ToTable("SearchAnalytics", Constants.Schema);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasMaxLength(128);
            
            builder.Property(x => x.Query).HasMaxLength(500).IsRequired();
            builder.Property(x => x.UserId).HasMaxLength(128);
            builder.Property(x => x.SearchedAt).IsRequired();
            builder.Property(x => x.ResultsCount).IsRequired();
            builder.Property(x => x.TimeTakenMs).IsRequired();
            builder.Property(x => x.Filters).HasMaxLength(2000);
            builder.Property(x => x.SearchType).HasMaxLength(50);
            builder.Property(x => x.HasResults).IsRequired();
            builder.Property(x => x.IpAddress).HasMaxLength(45);
            builder.Property(x => x.UserAgent).HasMaxLength(500);
            
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.SearchedAt);
            builder.HasIndex(x => x.Query);
            builder.HasIndex(x => x.SearchType);
        }
    }
}
