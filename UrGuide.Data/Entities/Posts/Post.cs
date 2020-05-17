using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            Itineraries = new HashSet<Itinerary>();
            Reservations = new HashSet<Reservation>();
            UserReactions = new HashSet<UserReaction>();
        }


        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public DateTime DateOfPublication { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public virtual ICollection<BidHistory> BidHistories { get; protected set; }
        public virtual ICollection<Feedback> Feedback { get; protected set; }
        public virtual ICollection<Itinerary> Itineraries { get; protected set; }
        public virtual ICollection<Reservation> Reservations { get; protected set; }
        public virtual ICollection<UserReaction> UserReactions { get; protected set; }
        public virtual Bid Bid { get; set; }
        public string CatalogRef { get; protected set; }
        public virtual ImageCatalog Catalog { get; set; }
        public virtual User User { get; set; }
        public virtual Point Location { get; set; }

        public bool IsPastDue => !DateTime.TryParse($"{Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.DateEnd)))} {Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.DateEnd)))}",
          CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.AssumeUniversal, out DateTime date) || date < DateTime.UtcNow; 

        public void NewBid(string value, User user)
        {
            if (IsPastDue)
            {
                throw new InvalidOperationException("This item has expired");
            }

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
                var priceAttr = Attributes.First(f => f.Name == nameof(AttributeTypes.Amount));
                Bid = new Bid
                {
                    NewValue = value,
                    Author = user,
                    LastUpdated = DateTime.UtcNow,
                    OldValue = priceAttr
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
            if (IsPastDue)
            {
                throw new InvalidOperationException("This item has expired");
            }

            if (Bid != null)
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
                }
                else
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
            if (IsPastDue)
            {
                throw new InvalidOperationException("This item has expired");
            }

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

        public void MakeReservation(string userId, int seats)
        {
            if(IsPastDue)
            {
                throw new InvalidOperationException("This item has expired");
            }

            if(Reservations.Any(r => r.UserId == userId))
            {
                throw new InvalidOperationException("A reservation already exists for this user.");
            }

            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (seats == 0)
            {
                throw new ArgumentException("You cannot reserve 0 seats.");
            }

            int seatsAvailable = Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.AllocatedSeats)));
            int reserved = Reservations.Sum(x => x.Seats);
            if (seatsAvailable == reserved)
            {
                throw new InvalidOperationException("This item is sold out");
            }

            if (seatsAvailable <= reserved + seats)
            {
                throw new InvalidOperationException("We can't allocated this many seats.");
            }

            Reservations.Add(new Reservation
            {
                UserId = userId,
                Seats = seats
            });

            var reservedSeats = Attributes.FirstOrDefault(a => a.Name.Equals(nameof(AttributeTypes.ReservedSeats)));
            if(reservedSeats == null)
            {
                reservedSeats = new GenericAttribute
                {
                    Name = nameof(AttributeTypes.ReservedSeats)
                };
                Attributes.Add(reservedSeats);
            }

            reservedSeats.Value = (reserved + seats).ToString();
        }

        public void EditReservation(string userId, int seats)
        {
            if (IsPastDue)
            {
                throw new InvalidOperationException("This item has expired");
            }

            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var reservation = Reservations.FirstOrDefault(r => r.UserId == userId);
            if (reservation == null)
            {
                throw new InvalidOperationException("A reservation doesn't exist for this user.");
            }

            if(seats == 0)
            {
                throw new ArgumentException("You cannot reserve 0 seats, cancel your reservation instead");
            }

            int seatsAvailable = Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.AllocatedSeats)));
            int reserved = Reservations.Sum(x => x.Seats) - reservation.Seats;

            if (seatsAvailable == reserved)
            {
                throw new InvalidOperationException("This item is sold out");
            }

            if (seatsAvailable <= reserved + seats)
            {
                throw new InvalidOperationException("We can't allocated this many seats.");
            }

            reservation.Seats = seats;

            var reservedSeats = Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.ReservedSeats)));
            reservedSeats.Value = (reserved + seats).ToString();
        }

        public void CancelReservation(string userId)
        {
            if (IsPastDue)
            {
                throw new InvalidOperationException("This item has expired");
            }

            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var reservation = Reservations.FirstOrDefault(r => r.UserId == userId);
            if (reservation == null)
            {
                throw new InvalidOperationException("A reservation doesn't exist for this user.");
            }

            int reserved = Reservations.Sum(x => x.Seats) - reservation.Seats;
            var reservedSeats = Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.ReservedSeats)));
            reservedSeats.Value = reserved.ToString();

            Reservations.Remove(reservation);
        }

        public void RecordUserReaction(string userId, UserReaction.ReactionType reactionType)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var userReaction = UserReactions.FirstOrDefault(u => u.UserId == userId);
            if (userReaction != null && (userReaction.Type & reactionType) == reactionType)
            {
                // the user already like or dislike this post, so we exit
                return;
            }
            UserReaction.ReactionType? previousReaction = null;
            if(userReaction == null)
            {
                UserReactions.Add(new UserReaction
                {
                    Type = reactionType,
                    UserId = userId
                });
            } 
            else
            {
                previousReaction = userReaction.Type;
                userReaction.Type = reactionType;
            }

            var likes = Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.Likes)));
            var dislikes = Attributes.First(a => a.Name.Equals(nameof(AttributeTypes.Dislikes)));

            int allLikes = likes;
            int disLikes = dislikes;

            switch (reactionType)
            {
                case UserReaction.ReactionType.Like:
                    allLikes++;
                    if (previousReaction.HasValue)
                        disLikes--;
                    break;
                case UserReaction.ReactionType.DisLike:
                    disLikes++;
                    if (previousReaction.HasValue)
                        allLikes--;
                    break;
                default:
                    break;
            }
            likes.Value = allLikes.ToString();
            dislikes.Value = disLikes.ToString();
        }
    }
}
