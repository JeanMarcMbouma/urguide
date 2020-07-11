using MvvmHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Rating : ContentView
    {

        private ICommand _tappedCommand;

        public ICommand TappedCommand => _tappedCommand ??= new Command<int>((value) =>
        {
            Value = value;
        }, (v) => IsEnabled);

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public int Value 
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public int StarWidthRequest
        {
            get { return (int)GetValue(StarWidthRequestProperty); }
            set { SetValue(StarWidthRequestProperty, value); }
        }


        public int StarHeightRequest
        {
            get { return (int)GetValue(StarHeightRequestProperty); }
            set { SetValue(StarHeightRequestProperty, value); }
        }
        public int Spacing
        {
            get { return (int)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create(nameof(Minimum), typeof(int), typeof(Rating), 0);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create(nameof(Maximum), typeof(int), typeof(Rating), 5);

        public static readonly BindableProperty StarHeightRequestProperty =
            BindableProperty.Create(nameof(StarHeightRequest), typeof(int), typeof(Rating), 24);

        public static readonly BindableProperty StarWidthRequestProperty =
            BindableProperty.Create(nameof(StarWidthRequest), typeof(int), typeof(Rating), 24);

        public static readonly BindableProperty ValueProperty =
            BindableProperty.Create(nameof(Value), typeof(int), typeof(Rating), 1, propertyChanged: (bindable, o,n) => {
                if(bindable is Rating rating)
                {
                    rating.Items.ReplaceRange(Enumerable.Range(1, rating.Maximum).Select(
                        x => new RatingModel
                        {
                            IsSelected = x <= rating.Value,
                            Value = x
                        }));
                } 
                else
                {

                }
            });

        public static readonly BindableProperty SpacingProperty =
            BindableProperty.Create(nameof(Spacing), typeof(int), typeof(Rating), 8);

        public ObservableRangeCollection<RatingModel> Items { get; set; } = new ObservableRangeCollection<RatingModel>();
        public Rating()
        {
            InitializeComponent();
            Items.AddRange(
            Enumerable.Range(1, Maximum).Select(
                        x => new RatingModel
                        {
                            IsSelected = x <= Value,
                            Value = x
                        }));
        }

    }

    public class RatingModel
    {
        public int Value { get; set; }
        public bool IsSelected { get; set; }
    }
}