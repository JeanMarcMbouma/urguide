using MvvmHelpers;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.ViewModels
{
    public class PostDetailViewModel : BaseViewModel
    {
        private PostItem selected;

        public PostItem Selected
        {
            get => selected; set
            {
                SetProperty(ref selected, value);
            }
        }
        public PostDetailViewModel()
        {

        }
    }
}
