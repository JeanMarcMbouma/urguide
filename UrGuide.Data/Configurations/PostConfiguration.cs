using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Configurations
{
    class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts", Constants.Schema);
            builder.HasKey(x => x.Id).HasName("PK_Posts");
            builder.Property(x => x.Id).HasColumnName("PostId")
                .HasDefaultValueSql(Constants.GuidFn);
            builder.Property(x => x.Text).HasColumnName("Title").HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.DateOfPublication).IsRequired();
            builder.Property(x => x.LastUpdated);
            builder.Property(x => x.Location);

            builder.Ignore(x => x.IsPastDue);

            builder.HasOne(x => x.User)
                .WithMany().HasForeignKey("UserId");

            builder.OwnsOne(x => x.Bid, b =>
            {
                b.ToTable("Post_Bids", Constants.Schema);
                b.WithOwner().HasForeignKey("PostId");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                b.Property(x => x.NewValue).IsRequired().HasMaxLength(200);
                b.Property(x => x.OldValue).HasMaxLength(200);
                b.Property(x => x.LastUpdated).IsRequired();
                b.HasOne(x => x.Author).WithMany().HasForeignKey("FK_Post_Bids_Users");
            });

            builder.OwnsMany(x => x.BidHistories, b =>
            {
                b.ToTable("Post_Bids_History", Constants.Schema);
                b.WithOwner().HasForeignKey("PostId");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                b.Property(x => x.Value).IsRequired().HasMaxLength(200);
                b.Property(x => x.Created).IsRequired();
                b.HasOne(x => x.Author).WithMany().HasForeignKey("FK_Post_Bids_History_Users");
            });

            builder.OwnsMany(x => x.Itineraries, b =>
            {
                b.ToTable("Post_Itineraries", Constants.Schema);
                b.WithOwner().HasForeignKey("PostId");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                b.Property(x => x.Title).IsRequired().HasMaxLength(100);
                b.Property(x => x.Description).IsRequired().HasMaxLength(500);
                b.Property(x => x.Ordinal).IsRequired().HasColumnType("tinyint");
            });

            builder.OwnsMany(x => x.Feedback, b =>
            {
                b.ToTable("Post_Feedback", Constants.Schema);
                b.WithOwner().HasForeignKey("PostId");
                b.HasKey(x => x.Id);
                b.Property(x => x.Id).HasDefaultValueSql(Constants.GuidFn);
                b.Property(x => x.Text).IsRequired().HasMaxLength(2000);
                b.Property(x => x.Created).IsRequired();
                b.Property(x => x.Rating).IsRequired();
                b.HasOne(x => x.Author).WithMany().HasForeignKey("FK_Post_Feedback_Users");
            });

            builder.OwnsMany(x => x.Attributes, a =>
            {
                a.ToTable("Post_Attributes", Constants.Schema);
                a.WithOwner().HasForeignKey("PostId");
                a.Property(x => x.Name).IsRequired().HasMaxLength(200);
                a.Property(x => x.Value).IsRequired();
                a.HasKey("Id");
            });

            builder.OwnsMany(x => x.Reservations, r => {
                r.ToTable("Seat_Reservations", Constants.Schema);
                r.WithOwner().HasForeignKey("PostId");
                r.Property(p => p.UserId).IsRequired();
                r.Property(p => p.Seats).IsRequired();
                r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade);
                r.HasKey("Id");
            });

            var converter = new ValueConverter<UserReaction.ReactionType, int>(
                v => (int)v,
                v => (UserReaction.ReactionType)v);

            builder.OwnsMany(x => x.UserReactions, r => {
                r.ToTable("Post_UserReactions", Constants.Schema);
                r.WithOwner().HasForeignKey("PostId");
                r.Property(p => p.UserId).IsRequired();
                r.Property(p => p.Type).HasConversion(converter).IsRequired();
                r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade);
                r.HasKey("Id");
            });
        }
    }
}
