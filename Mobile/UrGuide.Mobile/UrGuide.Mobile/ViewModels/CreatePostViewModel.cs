using FFImageLoading;
using MvvmHelpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.Views.Dialog;
using Xamarin.Essentials;

namespace UrGuide.Mobile.ViewModels
{
    class CreatePostViewModel : ReactiveObject
    {
        private readonly IFileService _fileService;
        private readonly INavigationService _navigation;
        private readonly PostItemCreationsQueue _creationsQueue;
        private CompositeDisposable _disposables = new CompositeDisposable();
        private DateTime date = DateTime.Today;
        private TimeSpan startTime = TimeSpan.FromHours(13);
        private TimeSpan endTime = TimeSpan.FromSeconds(18);
        private int priceFrom = 20;
        private int priceTo = 100;
        private int seats = 1;
        private string title;
        private string description;

        public ReactiveCommand<Unit, IEnumerable<FileResult>> UploadImageCommand { get; }
        public ReactiveCommand<Unit, Unit> UndoUploadImageCommand { get; }
        public ReactiveCommand<Unit, Unit> UndoItineraryCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateItineraryCommand { get; }

        public ReactiveCommand<Unit, Unit> PublishCommand { get; }
        public CreatePostViewModel(IPostItemService service,
            IFileService fileService,
            INavigationService navigation,
            PostItemCreationsQueue creationsQueue)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _creationsQueue = creationsQueue ?? throw new ArgumentNullException(nameof(creationsQueue));
            service.GetCategoriesAsync().ToObservable()
                .Where(x => !x.HasError)
                .Select(x => x.Data.Select(c => new SearchOption { Text = c.Name }))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(x => SearchOptions.ReplaceRange(x))
                .Subscribe();

            var canExecute = this.WhenAnyValue(x => x.Images.Count, x => x < 4);

            UploadImageCommand = ReactiveCommand
                .CreateFromObservable(_fileService.UploadImages, canExecute, RxApp.MainThreadScheduler);

            UploadImageCommand
                .Subscribe(fd =>
                {
                    if (fd != null)
                    {
                        fd.Take(4 - Images.Count).ToList().ForEach(async x =>
                        {
                            using var stream = await x.OpenReadAsync();
                            var dataArray = stream.ToByteArray();
                            var b64 = Convert.ToBase64String(dataArray);
                            MainThread.BeginInvokeOnMainThread(() =>
                            {

                                Images.Add(
                                new API.ImageFileModel
                                {
                                    Name = x.FileName,
                                    ImageBase64 = $"data:image/{System.IO.Path.GetExtension(x.FileName).Substring(1)};base64,{b64}"
                                });
                            });
                        });
                    }
                }).DisposeWith(_disposables);

            UploadImageCommand.ThrownExceptions.Subscribe(e =>
            {

            });

            var canUndoImageUplaod = this.WhenAnyValue(x => x.Images.Count, x =>
            {
                return x > 0;
            });

            UndoUploadImageCommand = ReactiveCommand.Create(() => Images.Clear(), canUndoImageUplaod, RxApp.MainThreadScheduler);

            UndoUploadImageCommand
                .Subscribe()
                .DisposeWith(_disposables);

            var canCreateItinerary = this.WhenAnyValue(x => x.Itineraries.Count, x =>
            {
                return x < 10;
            });

            CreateItineraryCommand = ReactiveCommand.Create(() =>
            {
                var page = new CreateItinerary
                {
                    BindingContext = new CreateItineraryViewModel(navigation, (it) =>
                    {
                        it.Ordinal = ItineraryCount + 1;
                        Itineraries.Add(it);
                        this.RaisePropertyChanged(nameof(ItineraryCount));
                    })
                };
                navigation.PushModalAsync(page);
            }, canCreateItinerary, RxApp.MainThreadScheduler);

            CreateItineraryCommand
                .Subscribe()
                .DisposeWith(_disposables);
            CreateItineraryCommand.ThrownExceptions.Subscribe(e =>
            {
                MainThread.BeginInvokeOnMainThread(async () => await navigation.DisplayErrorAsync(message: e.Message));
            });

            var canUndoIti = this.WhenAnyValue(x => x.Itineraries.Count, c => c > 0);
            UndoItineraryCommand = ReactiveCommand.Create(() =>
            {
                var last = Itineraries.Last();
                Itineraries.Remove(last);
                this.RaisePropertyChanged(nameof(ItineraryCount));
            }, canUndoIti, RxApp.MainThreadScheduler);

            UndoItineraryCommand.Subscribe().DisposeWith(_disposables);

            var canSubmit = this.WhenAnyValue(
                x => x.Title,
                x => x.Description,
                x => x.PriceFrom,
                x => x.PriceTo,
                x => x.Images,
                x => x.Itineraries,
                x => x.SearchOptions,
                (t, d, from, to, img, it, cat) =>
                   !string.IsNullOrEmpty(t)
                   && !string.IsNullOrEmpty(d)
                   && from < to
                   && img.Any()
                   && it.Any()
                   && cat.Any(c => c.Selected)
                );

            PublishCommand = ReactiveCommand.Create(Publish, canSubmit, RxApp.MainThreadScheduler);
            PublishCommand.Subscribe()
                .DisposeWith(_disposables);
            PublishCommand.ThrownExceptions.Subscribe(e =>
            {
                MainThread.BeginInvokeOnMainThread(async () => await navigation.DisplayErrorAsync(message: e.Message));
            });
        }

        void Publish()
        {
            var post = new API.PostCreationModel
            {
                Itineraries = Itineraries,
                BidOptIn = true,
                Categories = SearchOptions.Where(x => x.Selected).Select(x => x.Text).ToList(),
                EndDate = new DateTimeOffset(Date.Year, Date.Month, Date.Day, EndTime.Hours, EndTime.Minutes, EndTime.Seconds, TimeSpan.FromSeconds(0)),
                StartDate = new DateTimeOffset(Date.Year, Date.Month, Date.Day, StartTime.Hours, StartTime.Minutes, StartTime.Seconds, TimeSpan.FromSeconds(0)),
                Description = Description,
                Text = Title,
                Images = Images.Select(f => new API.ImageFileCreateModel
                {
                    ImageBase64 = f.ImageBase64,
                    Name = f.Name
                }).ToList(),
                Seats = Seats,
                UnitPrice = $"{PriceFrom}$ - {PriceTo}$",
                GeoLocation = GlobalSetting.Instance.City
            };
            _creationsQueue.OnNext(post);
            MainThread.BeginInvokeOnMainThread(async () => await _navigation.PopAsync());
        }

        public string Description { get => description; set => this.RaiseAndSetIfChanged(ref description, value); }
        public string Title { get => title; set => this.RaiseAndSetIfChanged(ref title, value); }
        public int PriceFrom { get => priceFrom; set => this.RaiseAndSetIfChanged(ref priceFrom, value); }
        public int PriceTo { get => priceTo; set => this.RaiseAndSetIfChanged(ref priceTo, value); }
        public int Seats { get => seats; set => this.RaiseAndSetIfChanged(ref seats, value); }
        public ObservableRangeCollection<API.ItineraryModel> Itineraries { get; set; } = new ObservableRangeCollection<API.ItineraryModel> { };
        public ObservableRangeCollection<API.ImageFileModel> Images { get; set; } =
            new ObservableRangeCollection<API.ImageFileModel>();
        public DateTime Date { get => date; set => this.RaiseAndSetIfChanged(ref date, value); }
        public DateTime MinDate { get; set; } = DateTime.Today;
        public DateTime MaxDate { get; set; } = DateTime.Today.AddDays(30);
        public TimeSpan StartTime { get => startTime; set => this.RaiseAndSetIfChanged(ref startTime, value); }
        public TimeSpan EndTime { get => endTime; set => this.RaiseAndSetIfChanged(ref endTime, value); }
        public int ItineraryCount => Itineraries.Count();
        public ObservableRangeCollection<SearchOption> SearchOptions { get; } = new ObservableRangeCollection<SearchOption>();
        public IPostItemService Service { get; }
    }

}