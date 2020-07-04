using MvvmHelpers;
using System;
using System.Collections.Generic;
using UrGuide.Model.Shared;
using Xamarin.Forms;

namespace UrGuide.Mobile.Models
{
    public class PostItem : ObservableObject
    {
        public PostItem()
        {
            Images = new HashSet<ImageFileModel>();
            Categories = new HashSet<string>();
        }
        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Rating { get; set; }
        public string Location { get; set; }
        public int Likes { get => likes; set => SetProperty(ref likes, value); }
        public int Dislikes { get => dislikes; set => SetProperty(ref dislikes, value); }
        public string PublicationDate { get; set; }
        public string LastEditDate { get; set; }
        public string StartingBid { get; set; }
        public string LastBid { get; set; }
        public string Status { get; set; }
        public int Seats { get; set; }
        public int ReservedSeats { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string StartTime { get; set; }
        public bool HasReserved { get; set; }
        public bool HasReacted { get; set; }
        public int ReactionType
        {
            get => reactionType;
            set => SetProperty(ref reactionType, value, onChanged: () =>
            {
                OnPropertyChanged(nameof(Likes));
                OnPropertyChanged(nameof(Dislikes));
            });
        }
        public int BidCount { get; set; }
        public int ItineraryCount { get; set; }
        public ICollection<string> Categories { get; protected set; }
        public ICollection<ImageFileModel> Images { get; protected set; }
        public string AuthorId { get; set; }
        public string Author { get; set; }
        public string AuthorAvatar { get; set; }
        public bool IsBidOptIn { get; set; }
        private PostItemCountdown _countdown;
        private int reactionType;
        private int likes;
        private int dislikes;

        public List<Model.Posts.ItineraryModel> Itineraries { get; set; }

        public PostItemCountdown Countdown => _countdown ??= new PostItemCountdown(StartDate, StartTime, EndDate, EndTime);

        public class PostItemCountdown : ObservableObject
        {
            public string Left => IsActive ? string.Format("   {0:dd} day(s) {0:hh\\:mm\\:ss}", TimeLeft) : string.Empty;
            public bool IsActive { get; set; }
            public TimeSpan TimeLeft
            {
                get; set;
            }

            public PostItemCountdown(string startdate, string starttime, string enddate, string endtime)
            {
                if (DateTime.TryParse($"{startdate} {starttime}", out var start) &&
                    DateTime.TryParse($"{enddate} {endtime}", out var end))
                {
                    IsActive = end > DateTime.UtcNow;
                    TimeLeft = (start - end).Negate();
                    Device.StartTimer(TimeSpan.FromSeconds(1), UpdateTimer);
                }
            }

            private bool UpdateTimer()
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    TimeLeft = TimeLeft.Subtract(TimeSpan.FromSeconds(1));
                    OnPropertyChanged(nameof(Left));
                });
                return TimeLeft.TotalSeconds > 0;
            }

            public override string ToString()
            {
                return Left;
            }
        }
    }
}
