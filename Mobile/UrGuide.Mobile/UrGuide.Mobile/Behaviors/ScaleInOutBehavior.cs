using System;
using Xamarin.Forms;
using Xamarin.Forms.Markup;

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


        private Animation _animation;
        View associatedObject;
        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            associatedObject = bindable;
            _animation = new Animation((scale) => {
                associatedObject.Scale = scale;
            }, Scale, 1, Easing.SinInOut, () => { });
            _animation.Commit(associatedObject, "ScaleInOut", length: 750, repeat: () => true);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            _animation = null;
            base.OnDetachingFrom(bindable);
            associatedObject = null;
        }
    }
}
