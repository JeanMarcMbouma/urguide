using AutoMapper;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.Extensions.DependencyInjection;
using System;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Mapping;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.ViewModels;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = "fas")]
namespace UrGuide.Mobile
{
    public static class Forms
    {
        public static IServiceProvider Ioc { get; private set; }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<API.PostsClient>(client => {
                client.BaseAddress = new Uri(Constants.BaseUrl);
            });
            services.AddHttpClient<API.FeedbackClient>(client => {
                client.BaseAddress = new Uri(Constants.BaseUrl);
            });
            services.AddHttpClient<API.LookupClient>(client => {
                client.BaseAddress = new Uri(Constants.BaseUrl);
            });

            services.AddSingleton<AppShell>();
            services.AddSingleton<IMainPageService, MainPageService>();
            services.AddSingleton<App>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPostItemService, PostItemService>();
            services.AddSingleton<IUserService, UserService>();


            services.AddScoped<LandingItemViewModel>();
            services.AddScoped<PostsViewModel>();
            services.AddScoped<PostDetailViewModel>();
            services.AddScoped<BidDialogViewModel>();
            services.AddScoped<FavoriteViewModel>();
            services.AddScoped<ProfileViewModel>();
            services.AddScoped<EditProfileViewModel>();
            services.AddScoped<ChangePasswordViewModel>();
            services.AddScoped<DiscoverViewModel>();

            services.AddAutoMapper(typeof(PostProfile).Assembly);
        }
        public static void Init(Action<IServiceCollection> registerServices)
        {
            Xamarin.Forms.Device.SetFlags(new[] { 
                "CollectionView_Experimental", 
                "Shapes_Experimental", 
                "CarouselView_Experimental",
                "Expander_Experimental",
                "FastRenderers_Experimental",
                "SwipeView_Experimental",
                "Markup_Experimental"
            });

            AppCenter.Start("android=c61793ee-ed79-4a3d-b023-15303f271bb4;" /*+
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}"*/,
                   typeof(Crashes), typeof(Analytics), typeof(Distribute));

            RegisterRoutes();
            var services = new ServiceCollection();
            ConfigureServices(services);
            registerServices?.Invoke(services);
            Ioc = services.BuildServiceProvider();
        }

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute("posts", typeof(PostPage));
            Routing.RegisterRoute("posts/details", typeof(PostDetailPage));
            Routing.RegisterRoute("postdetails", typeof(PostDetailPage));
            Routing.RegisterRoute("profile", typeof(Views.Profile));
            Routing.RegisterRoute("discover", typeof(Views.Discover));
        }
    }
}
