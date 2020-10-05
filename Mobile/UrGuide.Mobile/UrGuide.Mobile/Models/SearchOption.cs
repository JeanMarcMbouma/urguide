using MvvmHelpers;

namespace UrGuide.Mobile.Models
{
    public class SearchOption : ObservableObject
    {
        private bool selected;

        public string Text { get; set; }
        public bool Selected { get => selected; set => SetProperty(ref selected, value); }

        public static implicit operator SearchOption(string value) => new SearchOption
        {
            Text = value
        };
    }
}
