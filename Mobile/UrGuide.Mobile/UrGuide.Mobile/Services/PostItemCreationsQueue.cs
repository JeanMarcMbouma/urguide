using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using Xamarin.Forms;

namespace UrGuide.Mobile.Services
{
    class PostItemCreationsQueue : IObserver<API.PostCreationModel>
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public PostItemCreationsQueue(IPostItemService postItemService, PostItemsQueue itemsQueue)
        {
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            ItemsQueue = itemsQueue ?? throw new ArgumentNullException(nameof(itemsQueue));
        }

        public IPostItemService PostItemService { get; }
        public PostItemsQueue ItemsQueue { get; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(API.PostCreationModel value)
        {
            PostItemService.Create(value).Subscribe(ItemsQueue)
                .DisposeWith(disposables);
        }
    }

    public class PostItemsQueue : IObserver<PostItem>
    {
        public PostItemsQueue(INavigationService navigationService, IPostItemService posts)
        {
            Navigation = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public INavigationService Navigation { get; }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Navigation.DisplayErrorAsync(message: error.Message);
        }

        public void OnNext(PostItem value)
        {
            MessagingCenter.Instance.Send(this, nameof(PostItemsQueue), value);
        }
    }
}
