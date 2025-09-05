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

    public partial class TourRequestItem : ObservableObject
    {
        [ObservableProperty]
        private string tourRequestId = string.Empty;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime preferredDate;

        [ObservableProperty]
        private int maxParticipants;

        [ObservableProperty]
        private decimal maxBudget;

        [ObservableProperty]
        private string tags = string.Empty;

        [ObservableProperty]
        private DateTime createdAt;

        [ObservableProperty]
        private DateTime updatedAt;

        [ObservableProperty]
        private string status = string.Empty;

        [ObservableProperty]
        private string requesterId = string.Empty;

        [ObservableProperty]
        private string requesterName = string.Empty;

        [ObservableProperty]
        private string regionId = string.Empty;

        [ObservableProperty]
        private string regionName = string.Empty;

        // Display properties
        [ObservableProperty]
        private string formattedBudget = string.Empty;

        [ObservableProperty]
        private string formattedDate = string.Empty;

        [ObservableProperty]
        private bool canUpdateBudget;

        [ObservableProperty]
        private bool canCancel;
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

    public class CreateTourRequestModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime PreferredDate { get; set; }
        public int MaxParticipants { get; set; } = 1;
        public decimal MaxBudget { get; set; }
        public string Tags { get; set; } = string.Empty;
        public string RegionId { get; set; } = string.Empty;
    }

    public class UpdateBudgetModel
    {
        public decimal NewBudget { get; set; }
    }

    public class RegionModel
    {
        public string RegionId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CurrencyId { get; set; } = string.Empty;
    }
}