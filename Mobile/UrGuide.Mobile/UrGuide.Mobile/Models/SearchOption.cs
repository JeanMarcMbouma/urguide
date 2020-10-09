using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;

namespace UrGuide.Mobile.Models
{
    public class SearchOption : ObservableObject
    {
        private bool selected;
        private ICommand _selectedCommand;

        public string Text { get; set; }
        public bool Selected { get => selected; set => SetProperty(ref selected, value); }

        public ICommand SelectCommand => _selectedCommand ??= new Command(() => Selected = !Selected);
        public static implicit operator SearchOption(string value) => new SearchOption
        {
            Text = value
        };
    }
}
