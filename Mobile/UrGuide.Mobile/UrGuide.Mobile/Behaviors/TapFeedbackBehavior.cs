using Xamarin.Forms;
using Xamarin.Forms.Markup;

namespace UrGuide.Mobile.Behaviors
{
    class TapFeedbackBehavior : Behavior<View>
    {
        View associatedObject;
        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);
            associatedObject = bindable;
            _ = associatedObject.TapGesture((ges) =>
              {
                  ges.Tapped += Ges_Tapped;
                  
              });
        }

        private void Ges_Tapped(object sender, System.EventArgs e)
        {
            var animation = new Animation((scale) =>
            {
                associatedObject.Scale = scale;
            }, .9, 1, Easing.SpringOut);
            animation.Commit(associatedObject, nameof(TapFeedbackBehavior), length: 300);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            _ = associatedObject.TapGesture((ges) =>
            {
                ges.Tapped -= Ges_Tapped;

            });
            base.OnDetachingFrom(bindable);
            associatedObject = null;
        }
    }
}
