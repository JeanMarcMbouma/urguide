using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;

namespace UrGuide.Mobile.ViewModels
{
    class EditProfileViewModel : BaseViewModel
    {
        private ICommand _closeDialogCommand;
        private ICommand _loadCommand;
        private ICommand _saveCommand;
        private string firstName;
        private string lastName;
        private string country;
        private string city;
        private string profileImage;

        public ICommand CloseDialogCommand => _closeDialogCommand ??= new AsyncCommand(async () => await NavigationService.PopModalAsync());
        public ICommand LoadCommand => _loadCommand ??= new Command(() =>
        {
            var user = UserService.GetUserInfo();
            FirstName = user.FirstName;
            LastName = user.LastName;
            Country = user.Country;
            City = user.City;
            ProfileImage = user.ProfileImage;
        });
        public ICommand SaveCommand => _saveCommand ??= new Command(() => { });
        public EditProfileViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public INavigationService NavigationService { get; }
        public IUserService UserService { get; }

        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }
        public string Country { get => country; set => SetProperty(ref country, value); }
        public string City { get => city; set => SetProperty(ref city, value); }
        public string ProfileImage { get => profileImage; set => SetProperty(ref profileImage, value); }

    }
}
