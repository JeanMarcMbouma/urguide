using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Avatar : ContentView
    {


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(Avatar), ImageSource.FromUri(new Uri($"{GlobalSetting.DefaultEndpoint}/thumbs/00000000-0000-0000-0000-000000000000.png")),
                BindingMode.OneWay, propertyChanged: (b, o, n) =>
                {
                    if (b is Avatar avatar) {
                        avatar.image.Source = avatar.Source;
                    }
                });

        public Size Size
        {
            get { return (Size)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(Size), typeof(Avatar), new Size(48, 48),
                BindingMode.OneWay, propertyChanged: (b, o, n) =>
                {
                    if (b is Avatar avatar)
                    {
                        avatar.HeightRequest = avatar.Size.Height;
                        avatar.WidthRequest = avatar.Size.Width;
                    }
                });

        public string AuthorId
        {
            get { return (string)GetValue(AuthorIdProperty); }
            set { SetValue(AuthorIdProperty, value); }
        }

        public static readonly BindableProperty AuthorIdProperty =
            BindableProperty.Create(nameof(AuthorId), typeof(string), typeof(Avatar), null);

        public string AuthorFullName
        {
            get { return (string)GetValue(AuthorFullNameProperty); }
            set { SetValue(AuthorFullNameProperty, value); }
        }

        public static readonly BindableProperty AuthorFullNameProperty =
            BindableProperty.Create(nameof(AuthorFullName), typeof(string), typeof(Avatar), null);

        private IPreferenceService preference;
        private IPreferenceService Preference => preference ??= Forms.Ioc.GetService<IPreferenceService>();
        private INavigationService navigation;
        protected INavigationService Nav => navigation ??= Forms.Ioc.GetService<INavigationService>();
        private MainPageViewModel vm;
        private MainPageViewModel VM => vm ??= Forms.Ioc.GetService<MainPageViewModel>();
        public Avatar()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (Preference.UserId == AuthorId)
            {
                VM.GotoProfile();
            } else
            {
                await Nav.PushAsync(new ProfilePage(new ProfileInfo
                {
                    AuthorFullName = AuthorFullName,
                    AuthorId = AuthorId
                }));
            }
        }
    }
}