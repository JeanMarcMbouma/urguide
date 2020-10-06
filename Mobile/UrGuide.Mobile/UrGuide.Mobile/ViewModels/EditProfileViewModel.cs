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
        private string address;
        private string birthDay;
        private string description;
        private string gender;
        private string phone;

        public ICommand CloseDialogCommand => _closeDialogCommand ??= new AsyncCommand(async () => await NavigationService.PopModalAsync());
        public ICommand LoadCommand => _loadCommand ??= new Command(() =>
        {
            var user = UserService.CurrentUser;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Country = user.Country;
            City = user.City;
            ProfileImage = user.ProfileImage;
            BirthDay = user.BirthDay;
            Address = user.Address;
            Gender = user.Gender;
            Description = user.Description;
            Phone = user.PhoneNumber;
        });
        public ICommand SaveCommand => _saveCommand ??= new AsyncCommand(async () => {
            var r = UserService.SaveProfile(new Model.Users.UpdateGuideModel
            {
                BirthDay = BirthDay,
                Address = Address,
                City = City,
                Country = Country,
                Description = Description,
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                Phone = Phone,
                ProfileImage = ProfileImage
            });

            if (r.HasError)
                await NavigationService.DisplayErrorAsync(message: string.Join(Environment.NewLine, r.Errors));
            else
                await NavigationService.PopModalAsync();
        });
        public EditProfileViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public INavigationService NavigationService { get; }
        public IUserService UserService { get; }

        public bool IsGuide => UserService.IsGuide;
        public string FirstName { get => firstName; set => SetProperty(ref firstName, value); }
        public string Address { get => address; set => SetProperty(ref address, value); }
        public string Gender { get => gender; set => SetProperty(ref gender, value); }
        public string Phone { get => phone; set => SetProperty(ref phone, value); }
        public string BirthDay { get => birthDay; set => SetProperty(ref birthDay, value); }
        public string Description { get => description; set => SetProperty(ref description, value); }
        public string LastName { get => lastName; set => SetProperty(ref lastName, value); }
        public string Country { get => country; set => SetProperty(ref country, value); }
        public string City { get => city; set => SetProperty(ref city, value); }
        public string ProfileImage { get => profileImage; set => SetProperty(ref profileImage, value); }

    }
}
