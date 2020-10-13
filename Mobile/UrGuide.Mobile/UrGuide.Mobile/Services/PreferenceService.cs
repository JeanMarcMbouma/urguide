using ReactiveUI;

namespace UrGuide.Mobile.Services
{
    class PreferenceService : ReactiveObject, IPreferenceService
    {
        public string AuthToken
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(AuthToken), string.Empty);
            set
            {
                Xamarin.Essentials.Preferences.Set(nameof(AuthToken), value);
                this.RaisePropertyChanged();
            }
        }
        public string FullName
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(FullName), string.Empty);
            set
            {
                Xamarin.Essentials.Preferences.Set(nameof(FullName), value);
                this.RaisePropertyChanged();
            }
        }
        public string UserId
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(UserId), string.Empty);
            set
            {
                Xamarin.Essentials.Preferences.Set(nameof(UserId), value);
                this.RaisePropertyChanged();
            }
        }
        public string Image
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(Image), string.Empty);
            set => Xamarin.Essentials.Preferences.Set(nameof(Image), value);
        }
        public string Role
        {
            get => Xamarin.Essentials.Preferences.Get(nameof(Role), string.Empty);
            set => Xamarin.Essentials.Preferences.Set(nameof(Role), value);
        }
    }
}
