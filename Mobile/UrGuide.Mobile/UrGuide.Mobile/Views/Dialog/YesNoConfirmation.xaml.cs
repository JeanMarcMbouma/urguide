using System;
using System.Threading.Tasks;
using UrGuide.Mobile.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views.Dialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YesNoConfirmation : ContentPage
    {
        public Action<DialogResult> Callback { get; set; }
        public string DisplayText { get => (string)GetValue(DisplayTextProperty); set => SetValue(DisplayTextProperty, value); }

        public static BindableProperty DisplayTextProperty =
            BindableProperty.Create(nameof(DisplayText), typeof(string), typeof(YesNoConfirmation), "Are you sure?");
        public string YesText { get => (string)GetValue(YesTextProperty); set => SetValue(YesTextProperty, value); }

        public static BindableProperty YesTextProperty =
            BindableProperty.Create(nameof(YesText), typeof(string), typeof(YesNoConfirmation), "Yes");

        public string NoText { get => (string)GetValue(NoTextProperty); set => SetValue(NoTextProperty, value); }

        public static BindableProperty NoTextProperty =
            BindableProperty.Create(nameof(NoText), typeof(string), typeof(YesNoConfirmation), "No");


        public bool DisplayNoButton { get => (bool)GetValue(DisplayNoButtonProperty); set => SetValue(DisplayNoButtonProperty, value); }

        public static BindableProperty DisplayNoButtonProperty =
            BindableProperty.Create(nameof(DisplayNoButton), typeof(bool), typeof(YesNoConfirmation), true);
        
        private DialogResult _result;

        public YesNoConfirmation()
        {
            InitializeComponent();
            _result = DialogResult.Unknown;
        }

        private async void Yes_Tapped(object sender, EventArgs e)
        {
            _result = DialogResult.Yes;
            await Close();
        }

        private async void No_Tapped(object sender, EventArgs e)
        {
            _result = DialogResult.No;
            await Close();
        }

        private async void Close_Tapped(object sender, EventArgs e)
        {
            await Close();
        }

        private async Task Close()
        {
            Callback?.Invoke(_result);
            await Navigation.PopModalAsync();
            Callback = null;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}