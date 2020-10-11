using System;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Profile : ContentView
    {
        public Profile()
        {
            InitializeComponent();
        }

        private void Close_Expander(object sender, EventArgs e)
        {
            floatingButton.IsExpanded = false;
            UpdateFloatingButtonBounds();
        }

        private void Expander_Tapped(object sender, EventArgs e)
        {
            UpdateFloatingButtonBounds();
        }

        private void UpdateFloatingButtonBounds()
        {
            var bounds = AbsoluteLayout.GetLayoutBounds(floatingButton);
            var originalHeight = bounds.Height;
            var isExpanding = bounds.Height < 400;
            var newHeight = isExpanding ? 400 : 170;
            var easing = isExpanding ? floatingButton.ExpandAnimationEasing : floatingButton.CollapseAnimationEasing;
            var length = isExpanding ? floatingButton.ExpandAnimationLength : floatingButton.CollapseAnimationLength;
            var animation = new Animation((v) =>
            {
                bounds.Height = v;
                AbsoluteLayout.SetLayoutBounds(floatingButton, bounds);
            }, originalHeight, newHeight, easing);
            animation.Commit(floatingButton, "SetLayoutBounds", length: length);
        }
    }
}