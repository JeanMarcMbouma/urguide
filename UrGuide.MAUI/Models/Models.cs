using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using UrGuide.Model.Shared;

namespace UrGuide.MAUI.Models
{
    public partial class PostItem : ObservableObject
    {
        [ObservableProperty]
        private string id = string.Empty;

        [ObservableProperty]
        private string text = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string price = string.Empty;

        [ObservableProperty]
        private string rating = string.Empty;

        [ObservableProperty]
        private string location = string.Empty;

        [ObservableProperty]
        private int likes;

        [ObservableProperty]
        private int dislikes;

        [ObservableProperty]
        private string publicationDate = string.Empty;

        [ObservableProperty]
        private string lastBid = string.Empty;

        [ObservableProperty]
        private int seats;

        [ObservableProperty]
        private int reservedSeats;

        [ObservableProperty]
        private string startDate = string.Empty;

        [ObservableProperty]
        private string endDate = string.Empty;

        [ObservableProperty]
        private string endTime = string.Empty;

        [ObservableProperty]
        private string startTime = string.Empty;

        [ObservableProperty]
        private bool hasReserved;

        [ObservableProperty]
        private bool hasReacted;

        [ObservableProperty]
        private int reactionType;

        [ObservableProperty]
        private int bidCount;

        [ObservableProperty]
        private int itineraryCount;

        [ObservableProperty]
        private string authorId = string.Empty;

        [ObservableProperty]
        private string author = string.Empty;

        [ObservableProperty]
        private string authorAvatar = string.Empty;

        [ObservableProperty]
        private bool isBidOptIn;

        [ObservableProperty]
        private int reviews;

        [ObservableProperty]
        private bool favorite;

        [ObservableProperty]
        private string status = string.Empty;

        // Additional properties for display
        [ObservableProperty]
        private string title = string.Empty;

        public List<string> Categories { get; set; } = new();
        public List<ImageFileModel> Images { get; set; } = new();
        public ObservableCollection<AuthoredFeedback> FeedBack { get; set; } = new();
    }

    public partial class DiscoverItem : ObservableObject
    {
        [ObservableProperty]
        private string id = string.Empty;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string price = string.Empty;

        [ObservableProperty]
        private string location = string.Empty;

        [ObservableProperty]
        private string imageUrl = string.Empty;
    }
}

namespace UrGuide.MAUI.Models.API
{
    public class PostCreationModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Additional properties will be migrated
    }
}