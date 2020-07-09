using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using static UrGuide.Mobile.ViewModels.ProfileViewModel;

namespace UrGuide.Mobile.Views.Templates.Profile
{
    class ProfileModeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PostsTemplate { get; set; }
        public DataTemplate GalleryTemplate { get; set; }
        public DataTemplate ReviewsTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            ProfileDisplayMode mode = (item as WrapperViewModel)?.Mode ?? ProfileDisplayMode.Reviews;
            return mode switch
            {
                ProfileDisplayMode.Reviews => ReviewsTemplate,
                ProfileDisplayMode.Posts => PostsTemplate,
                ProfileDisplayMode.Gallery => GalleryTemplate,
                _ => null
            };
        }
    }
}
