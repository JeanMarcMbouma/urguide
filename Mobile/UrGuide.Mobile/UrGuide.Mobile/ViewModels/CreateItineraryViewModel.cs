using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using UrGuide.Mobile.API;
using UrGuide.Mobile.Contracts;
using Xamarin.Essentials;

namespace UrGuide.Mobile.ViewModels
{
    class CreateItineraryViewModel : ReactiveObject

    {
        private CompositeDisposable _disposables = new CompositeDisposable();
        private string title;
        private string description;

        public CreateItineraryViewModel(INavigationService navigation, Action<ItineraryModel> createFn)
        {
            CreateFn = createFn ?? throw new ArgumentNullException(nameof(createFn));

            var canExecute = this.WhenAnyValue(x => x.Title, x => x.Description,
                (t, d) => !string.IsNullOrEmpty(t) && !string.IsNullOrEmpty(d));
            SaveCommand = ReactiveCommand.Create(() =>
            {
                CreateFn(new ItineraryModel
                {
                    Description = Description,
                    Title = Title
                });
                navigation.PopModalAsync();
            }, canExecute, RxApp.MainThreadScheduler);
            SaveCommand.Subscribe().DisposeWith(_disposables);
            SaveCommand.ThrownExceptions.Subscribe(e =>
            MainThread.BeginInvokeOnMainThread(async () => await navigation.DisplayErrorAsync(message: e.Message)));
            CloseDialogCommand = ReactiveCommand.Create(() =>
            {
                navigation.PopModalAsync();
            });
            CloseDialogCommand.Subscribe().DisposeWith(_disposables);
        }
        public string Title
        {
            get => title; set
            {
                this.RaiseAndSetIfChanged(ref title, value);
            }
        }
        public string Description
        {
            get => description; set
            {
                this.RaiseAndSetIfChanged(ref description, value);

            }
        }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CloseDialogCommand { get; }
        public Action<ItineraryModel> CreateFn { get; }
    }
}
