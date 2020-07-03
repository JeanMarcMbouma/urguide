using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace UrGuide.Mobile.Models
{
    public class PostItem : Model.Posts.PostModel
    {
        private PostItemCountdown _countdown;
        public List<Model.Posts.ItineraryModel> Itineraries { get; set; }

        public PostItemCountdown Countdown => _countdown ?? (_countdown = new PostItemCountdown(StartDate, StartTime, EndDate, EndTime));
        public class PostItemCountdown : ObservableObject
        {
            public string Left => IsActive ? string.Format("   {0:dd} day(s) {0:hh\\:mm\\:ss}", TimeLeft) : string.Empty;
            public bool IsActive { get; set; }
            public TimeSpan TimeLeft
            {
                get;set;
            }

            public PostItemCountdown(string startdate, string starttime, string enddate, string endtime)
            {
                if(DateTime.TryParse($"{startdate} {starttime}", out var start) && 
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
