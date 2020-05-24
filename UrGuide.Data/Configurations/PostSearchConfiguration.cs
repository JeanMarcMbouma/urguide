using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using UrGuide.Data.Entities.Posts;

namespace UrGuide.Data.Configurations
{
    class PostSearchConfiguration : IEntityTypeConfiguration<PostSearch>
    {
        public void Configure(EntityTypeBuilder<PostSearch> builder)
        {
            builder.ToView("Post_Search", Constants.Schema);
            builder.HasNoKey();
            builder.Property(x => x.PostId);
            builder.Property(x => x.EndDate);
            builder.Property(x => x.Location);
            builder.Property(x => x.Rating);
        }
    }
}
