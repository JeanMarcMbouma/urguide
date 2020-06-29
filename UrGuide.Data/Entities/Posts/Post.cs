using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Data.Entities.Shared;
using UrGuide.Data.Entities.Users;
using UrGuide.Data.Shared;

namespace UrGuide.Data.Entities.Posts
{
    public class Post : IUserOwnedEntity, IGeoEntity, ILastUpdatableEntity
    {
        public Post()
        {
            BidHistories = new HashSet<BidHistory>();
            Feedback = new HashSet<Feedback>();
            Itineraries = new HashSet<Itinerary>();
            Reservations = new HashSet<Reservation>();
            UserReactions = new HashSet<UserReaction>();
        }


        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool BidEnabled { get; set; }
        public int Rating { get; set; }
        public int Reviews { get; set; }
        public int AllocatedSeats { get; set; }
        public int ReservedSeats { get; set; }
        public string GeoLocation { get; set; }
        public string Tags { get; set; }
        public string Cost { get; set; }
        public int BidCount { get; set; }
        public int ItineraryCount { get; set; }
        public string LastBid { get; set; }

        public DateTime DateOfPublication { get; set; }
        public DateTime LastUpdated { get; set; }

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

        public bool IsPastDue => EndDate.HasValue && EndDate.Value <  DateTime.UtcNow; 

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
                Bid = new Bid
                {
                    NewValue = value,
                    Author = user,
                    LastUpdated = DateTime.UtcNow,
                    OldValue = Cost
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

            LastBid = Bid.NewValue;
            BidCount++;
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
                Bid.OldValue = Cost;
                LastBid = Cost;
                Cost = Bid.NewValue;
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

            int reserved = Reservations.Sum(x => x.Seats);
            if (ReservedSeats == reserved)
            {
                throw new InvalidOperationException("This item is sold out");
            }

            if (ReservedSeats <= reserved + seats)
            {
                throw new InvalidOperationException("We can't allocated this many seats.");
            }

            Reservations.Add(new Reservation
            {
                UserId = userId,
                Seats = seats
            });

            ReservedSeats = (reserved + seats);
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

            int reserved = Reservations.Sum(x => x.Seats) - reservation.Seats;

            if (ReservedSeats == reserved)
            {
                throw new InvalidOperationException("This item is sold out");
            }

            if (ReservedSeats <= reserved + seats)
            {
                throw new InvalidOperationException("We can't allocated this many seats.");
            }

            reservation.Seats = seats;

            ReservedSeats = (reserved + seats);
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

            ReservedSeats = Reservations.Sum(x => x.Seats) - reservation.Seats;
            Reservations.Remove(reservation);
        }

        public void RecordUserReaction(string userId, UserReaction.ReactionType reactionType)
        {
            if (userId is null)
            {
                throw new ArgumentNullException(nameof(userId));
            }


            var userReaction = UserReactions.FirstOrDefault(u => u.UserId == userId);
            if (userReaction != null && (userReaction.Type == reactionType))
            {
                // the user already like or dislike this post, so we exit
                UserReactions.Remove(userReaction);
               
                if(reactionType == UserReaction.ReactionType.Like)
                {
                    Likes--;
                }
                else
                {
                    Dislikes--;
                }
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


            switch (reactionType)
            {
                case UserReaction.ReactionType.Like:
                    Likes++;
                    if (previousReaction.HasValue)
                        Dislikes--;
                    break;
                case UserReaction.ReactionType.DisLike:
                    Dislikes++;
                    if (previousReaction.HasValue)
                        Likes--;
                    break;
                default:
                    break;
            }
        }
    }
}
