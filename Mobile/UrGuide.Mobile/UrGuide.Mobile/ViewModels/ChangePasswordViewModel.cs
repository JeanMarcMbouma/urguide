using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;

namespace UrGuide.Mobile.ViewModels
{
    class ChangePasswordViewModel : BaseViewModel
    {
        private string _oldPassword;
        private string _newPassword;
        private string _confirmPassword;
        private ICommand _changePasswordCommand;
        private AsyncCommand _closeDialogCommand;

        public ChangePasswordViewModel(INavigationService navigationService, IUserService userService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public ICommand CloseDialogCommand => _closeDialogCommand ??= new AsyncCommand(async () => await NavigationService.PopModalAsync());
        public ICommand ChangePasswordCommand => _changePasswordCommand ??= new AsyncCommand(async () =>
        {
            var r = UserService.ChangePassword(new Model.Users.ChangePasswordModel
            {
                Password = NewPassword,
                CurrentPassword = OldPassword,
                ConfirmPassword = ConfirmPassword
            });
            if (r.HasError)
            {
                await NavigationService.DisplayErrorAsync(message: string.Join(Environment.NewLine, r.Errors));
            } 
            else
            {
                await NavigationService.PopModalAsync();
            }
        });

        public string OldPassword { get => _oldPassword; set => SetProperty(ref _oldPassword, value); }
        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }
        public string ConfirmPassword { get => _confirmPassword; set => SetProperty(ref _confirmPassword, value); }
        public INavigationService NavigationService { get; }
        public IUserService UserService { get; }
    }
}
