using System;
using Xamarin.Forms;

namespace UrGuide.Mobile.Behaviors
{
    class ScaleInOutBehavior : Behavior<View>
    {


        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly  BindableProperty ScaleProperty =
            BindableProperty.Create(nameof(Scale), typeof(double), typeof(ScaleInOutBehavior), 1.0);


        bool isAnimating = false;
        View associatedObject;
        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            associatedObject = bindable;
            isAnimating = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), Redraw);
        }

        private bool Redraw()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await associatedObject.ScaleTo(Scale, 250, Easing.CubicInOut);
                await associatedObject.ScaleTo(1, 250, Easing.CubicInOut);
            });
            return isAnimating;
        }

        protected override void OnDetachingFrom(View bindable)
        {
            isAnimating = false;
            base.OnDetachingFrom(bindable);
            associatedObject = null;
        }
    }
}
