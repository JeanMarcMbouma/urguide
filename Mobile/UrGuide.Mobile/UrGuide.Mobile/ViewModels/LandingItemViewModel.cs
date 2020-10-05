using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.ViewModels
{
    class LandingItemViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        public ObservableRangeCollection<LandingItem> Items { get; }

        public ICommand SkipCommand { get; }
        public LandingItemViewModel(INavigationService navigation)
        {
            Items = new ObservableRangeCollection<LandingItem> { 
                new LandingItem
                {
                    ImageUrl = "http://urguide.azurewebsites.net/logo/android-chrome-512x512.png",
                    Header = "UrGuide",
                    Description = "Be a tourism guide at your ease and pace"
                },
                new LandingItem
                {
                    ImageUrl = "http://urguide.azurewebsites.net/images/extreme.png",
                    Header = "Extreme",
                    Description = "Get the adrenaline you've been looking for"
                },
                new LandingItem
                {
                    ImageUrl = "http://urguide.azurewebsites.net/images/sport.png",
                    Header = "Sport",
                    Description = "Keep the sport attitude"
                }
            };
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            SkipCommand = new AsyncCommand(async () =>
            {
                await _navigation.GotoAsync("posts");
            });
        }
    }
}
