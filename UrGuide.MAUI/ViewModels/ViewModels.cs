using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UrGuide.MAUI.Contracts;
using UrGuide.MAUI.Models;
using UrGuide.MAUI.Services;

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
        private void RemoveFavorite(PostItem item)
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

    public partial class CreateTourRequestViewModel : ObservableObject
    {
        private readonly ITourRequestService _tourRequestService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string title = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime preferredDate = DateTime.Today.AddDays(1);

        [ObservableProperty]
        private int maxParticipants = 1;

        [ObservableProperty]
        private decimal maxBudget;

        [ObservableProperty]
        private string tags = string.Empty;

        [ObservableProperty]
        private UrGuide.MAUI.Models.API.RegionModel selectedRegion;

        [ObservableProperty]
        private ObservableCollection<UrGuide.MAUI.Models.API.RegionModel> regions = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public CreateTourRequestViewModel(ITourRequestService tourRequestService, INavigationService navigationService)
        {
            _tourRequestService = tourRequestService;
            _navigationService = navigationService;
            LoadRegionsCommand = new RelayCommand(async () => await LoadRegions());
            CreateTourRequestCommand = new RelayCommand(async () => await CreateTourRequest(), CanCreateTourRequest);
        }

        public RelayCommand LoadRegionsCommand { get; }
        public RelayCommand CreateTourRequestCommand { get; }

        private async Task LoadRegions()
        {
            try
            {
                IsLoading = true;
                var result = await _tourRequestService.GetRegionsAsync();
                if (result?.Data != null)
                {
                    Regions.Clear();
                    foreach (var region in result.Data)
                    {
                        Regions.Add(region);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load regions: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CreateTourRequest()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var model = new UrGuide.MAUI.Models.API.CreateTourRequestModel
                {
                    Title = Title,
                    Description = Description,
                    PreferredDate = PreferredDate,
                    MaxParticipants = MaxParticipants,
                    MaxBudget = MaxBudget,
                    Tags = Tags,
                    RegionId = SelectedRegion?.RegionId ?? string.Empty
                };

                var result = await _tourRequestService.CreateTourRequestAsync(model);
                if (result != null && result.Data != null)
                {
                    // Reset form
                    Title = string.Empty;
                    Description = string.Empty;
                    PreferredDate = DateTime.Today.AddDays(1);
                    MaxParticipants = 1;
                    MaxBudget = 0;
                    Tags = string.Empty;
                    SelectedRegion = null;

                    // Navigate back or show success message
                    await _navigationService.PopAsync();
                }
                else
                {
                    ErrorMessage = "Failed to create tour request";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanCreateTourRequest()
        {
            return !IsLoading &&
                   !string.IsNullOrWhiteSpace(Title) &&
                   !string.IsNullOrWhiteSpace(Description) &&
                   SelectedRegion != null &&
                   MaxBudget > 0 &&
                   MaxParticipants > 0;
        }

        partial void OnTitleChanged(string value) => CreateTourRequestCommand.NotifyCanExecuteChanged();
        partial void OnDescriptionChanged(string value) => CreateTourRequestCommand.NotifyCanExecuteChanged();
        partial void OnSelectedRegionChanged(UrGuide.MAUI.Models.API.RegionModel value) => CreateTourRequestCommand.NotifyCanExecuteChanged();
        partial void OnMaxBudgetChanged(decimal value) => CreateTourRequestCommand.NotifyCanExecuteChanged();
        partial void OnMaxParticipantsChanged(int value) => CreateTourRequestCommand.NotifyCanExecuteChanged();
    }

    public partial class MyTourRequestsViewModel : ObservableObject
    {
        private readonly ITourRequestService _tourRequestService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<TourRequestItem> tourRequests = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasNoTourRequests = true;

        public MyTourRequestsViewModel(ITourRequestService tourRequestService, INavigationService navigationService)
        {
            _tourRequestService = tourRequestService;
            _navigationService = navigationService;
            LoadTourRequestsCommand = new RelayCommand(async () => await LoadTourRequests());
            CreateNewRequestCommand = new RelayCommand(async () => await CreateNewRequest());
        }

        public RelayCommand LoadTourRequestsCommand { get; }
        public RelayCommand CreateNewRequestCommand { get; }

        private async Task LoadTourRequests()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var result = await _tourRequestService.GetMyTourRequestsAsync();
                if (result?.Data != null)
                {
                    TourRequests.Clear();
                    foreach (var request in result.Data)
                    {
                        TourRequests.Add(request);
                    }
                    HasNoTourRequests = !TourRequests.Any();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load tour requests: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task CreateNewRequest()
        {
            await _navigationService.PushAsync("CreateTourRequestPage");
        }

        [RelayCommand]
        private async Task UpdateBudget(TourRequestItem tourRequest)
        {
            // TODO: Implement proper parameter passing for UpdateBudgetPage
            // Currently the UpdateBudgetViewModel expects a TourRequestItem parameter
            // but the navigation system doesn't support parameter passing yet.
            // This would need to be implemented with a proper navigation framework
            // or shared state management.
            await _navigationService.PushAsync("UpdateBudgetPage");
        }

        [RelayCommand]
        private async Task CancelRequest(TourRequestItem tourRequest)
        {
            try
            {
                await _navigationService.ConfirmAsync(async (result) =>
                {
                    if (result == DialogResult.Yes)
                    {
                        var cancelResult = await _tourRequestService.CancelTourRequestAsync(tourRequest.TourRequestId);
                        if (cancelResult?.Data == true)
                        {
                            TourRequests.Remove(tourRequest);
                            HasNoTourRequests = !TourRequests.Any();
                        }
                    }
                }, "Confirm", "Are you sure you want to cancel this tour request?");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to cancel request: {ex.Message}";
            }
        }
    }

    public partial class UpdateBudgetViewModel : ObservableObject
    {
        private readonly ITourRequestService _tourRequestService;
        private readonly INavigationService _navigationService;
        private readonly TourRequestItem _tourRequest;

        [ObservableProperty]
        private decimal currentBudget;

        [ObservableProperty]
        private decimal newBudget;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public UpdateBudgetViewModel(ITourRequestService tourRequestService, INavigationService navigationService, TourRequestItem tourRequest)
        {
            _tourRequestService = tourRequestService;
            _navigationService = navigationService;
            _tourRequest = tourRequest;
            CurrentBudget = tourRequest.MaxBudget;
            NewBudget = tourRequest.MaxBudget;
            UpdateBudgetCommand = new RelayCommand(async () => await UpdateBudget(), CanUpdateBudget);
        }

        public RelayCommand UpdateBudgetCommand { get; }

        private async Task UpdateBudget()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var result = await _tourRequestService.UpdateBudgetAsync(_tourRequest.TourRequestId, NewBudget);
                if (result?.Data != null)
                {
                    await _navigationService.PopAsync();
                }
                else
                {
                    ErrorMessage = "Failed to update budget";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanUpdateBudget()
        {
            return !IsLoading && NewBudget > CurrentBudget;
        }

        partial void OnNewBudgetChanged(decimal value) => UpdateBudgetCommand.NotifyCanExecuteChanged();
    }
}