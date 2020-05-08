using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using UrGuide.Data.Entities.Attributes;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Data.Entities.Shared;
using UrGuide.Data.Entities.Users;
using UrGuide.Data.Shared;

namespace UrGuide.Data.Entities.Posts
{
    public class Post : IAttributeEnabledEntity, IUserOwnedEntity, IGeoEntity, ILastUpdatableEntity
    {
        public Post()
        {
            Attributes = new HashSet<GenericAttribute>();
            BidHistories = new HashSet<BidHistory>();
            Feedback = new HashSet<Feedback>();
        }


        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public DateTime DateOfPublication { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public virtual ICollection<BidHistory> BidHistories { get; protected set; }
        public virtual ICollection<Feedback> Feedback { get; protected set; }
        public virtual Bid Bid { get; set; }
        public virtual ImageCatalog Catalog { get; set; }
        public virtual User User { get; set; }
        public virtual Point Location { get; set; }


        public void NewBid(string value, User user)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("message", nameof(value));
            }

            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (Bid == null)
            {
                Bid = new Bid
                {
                    NewValue = value,
                    Author = user,
                    LastUpdated = DateTime.UtcNow
                };
            } 
            else
            {
                var newHistory = new BidHistory
                {
                    Created = DateTime.UtcNow,
                    Author = Bid.Author,
                    Value = Bid.NewValue
                };

                Bid.OldValue = newHistory.Value;
                Bid.NewValue = value;
                Bid.LastUpdated = DateTime.UtcNow;
                BidHistories.Add(newHistory);
            }
        }

        public void AcceptBid()
        {
            if(Bid != null)
            {
                var history = new BidHistory
                {
                    Author = Bid.Author,
                    Created = Bid.LastUpdated,
                    Value = Bid.NewValue
                };

                BidHistories.Add(history);
                var priceAttr = Attributes.First(f => f.Name == nameof(AttributeTypes.Amount));
                var lastBid = Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.LastBid));
                Bid.OldValue = priceAttr.Value;

                if (lastBid != null)
                {
                    lastBid.Value = Bid.OldValue;
                } else
                {
                    Attributes.Add(new GenericAttribute
                    {
                        Name = nameof(AttributeTypes.LastBid),
                        Value = Bid.OldValue
                    });
                }
                priceAttr.Value = Bid.NewValue;
            } 
            else
            {
                throw new InvalidOperationException("You cannot accept an empty bid");
            }
        }

        public void RejectBid()
        {
            if (Bid != null)
            {
                var history = new BidHistory
                {
                    Author = Bid.Author,
                    Created = Bid.LastUpdated,
                    Value = Bid.NewValue
                };

                BidHistories.Add(history);
                Bid = null;
            }
            else
            {
                throw new InvalidOperationException("You cannot reject an empty bid");
            }
        }
    }
}
