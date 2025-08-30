using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UrGuide.MAUI.Contracts;
using UrGuide.MAUI.Models;

namespace UrGuide.MAUI.ViewModels
{
    public partial class DiscoverViewModel : ObservableObject
    {
        private readonly IPostItemService _postItemService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private ObservableCollection<DiscoverItem> posts = new();

        public DiscoverViewModel(IPostItemService postItemService, INavigationService navigationService)
        {
            _postItemService = postItemService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task Search()
        {
            if (IsLoading) return;

            IsLoading = true;
            try
            {
                var result = await _postItemService.SearchAsync(false, null, SearchText);
                if (result?.Data != null)
                {
                    Posts.Clear();
                    foreach (var item in result.Data)
                    {
                        Posts.Add(item);
                    }
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    public partial class PostsViewModel : ObservableObject
    {
        private readonly IPostItemService _postItemService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private ObservableCollection<PostItem> posts = new();

        public PostsViewModel(IPostItemService postItemService, INavigationService navigationService)
        {
            _postItemService = postItemService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task CreatePost()
        {
            // Navigate to create post page
            await _navigationService.PushAsync("CreatePostPage");
        }
    }

    public partial class FavoriteViewModel : ObservableObject
    {
        private readonly IPostItemService _postItemService;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private ObservableCollection<PostItem> favorites = new();

        [ObservableProperty]
        private bool hasNoFavorites = true;

        public FavoriteViewModel(IPostItemService postItemService)
        {
            _postItemService = postItemService;
        }

        [RelayCommand]
        private async Task RemoveFavorite(PostItem item)
        {
            if (item != null)
            {
                Favorites.Remove(item);
                HasNoFavorites = !Favorites.Any();
            }
        }
    }

    public partial class ProfileViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string userName = "User";

        [ObservableProperty]
        private string email = "user@example.com";

        [ObservableProperty]
        private string userInitials = "U";

        [ObservableProperty]
        private int postsCount;

        [ObservableProperty]
        private int reviewsCount;

        [ObservableProperty]
        private double rating = 4.5;

        public ProfileViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private async Task EditProfile()
        {
            await _navigationService.PushAsync("EditProfilePage");
        }

        [RelayCommand]
        private async Task ChangePassword()
        {
            await _navigationService.PushAsync("ChangePasswordPage");
        }

        [RelayCommand]
        private async Task Settings()
        {
            await _navigationService.PushAsync("SettingsPage");
        }

        [RelayCommand]
        private async Task Logout()
        {
            // Implement logout logic
            await _navigationService.PushAsync("LoginPage");
        }
    }

    public partial class PostDetailViewModel : ObservableObject
    {
        [ObservableProperty]
        private PostItem currentPost;
    }

    public partial class BidDialogViewModel : ObservableObject
    {
        [ObservableProperty]
        private decimal bidAmount;
    }

    public partial class EditProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;
    }

    public partial class ChangePasswordViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentPassword = string.Empty;

        [ObservableProperty]
        private string newPassword = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;
    }

    public partial class ShellViewModel : ObservableObject
    {
    }

    public partial class MainPageViewModel : ObservableObject
    {
    }

    public partial class CreatePostViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private decimal price;
    }
}