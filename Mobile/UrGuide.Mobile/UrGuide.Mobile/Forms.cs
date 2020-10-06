using Akavache;
using AutoMapper;
using IdentityModel.Client;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Mapping;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.Services.Identity;
using UrGuide.Mobile.ViewModels;
using UrGuide.Mobile.Views;
using Xamarin.Forms;
using Akavache.Sqlite3;

[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = "fas")]
[assembly: ExportFont("Font Awesome 5 Free-Regular-400.otf", Alias = "far")]
namespace UrGuide.Mobile
{
    public static class Forms
    {
        public static IServiceProvider Ioc { get; private set; }
        public static void ConfigureServices(IServiceCollection services)
        {
            var clientRegistration = new Action<IServiceProvider, HttpClient>((ioc, client) =>
            {
                var pref = ioc.GetRequiredService<IPreferenceService>();
                client.BaseAddress = new Uri(GlobalSetting.DefaultEndpoint);
                string authToken = pref.AuthToken;
                if (!string.IsNullOrEmpty(authToken))
                    client.SetBearerToken(authToken);
            });
            services.AddHttpClient<API.PostsClient>(nameof(API.PostsClient), clientRegistration);
            services.AddHttpClient<API.FeedbackClient>(clientRegistration);
            services.AddHttpClient<API.LookupClient>(clientRegistration);
            services.AddHttpClient<API.BidClient>(clientRegistration);
            
            services.AddSingleton<AppShell>();
            services.AddSingleton<IMainPageService, MainPageService>();
            services.AddSingleton<App>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPostItemService, PostItemService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IPreferenceService, PreferenceService>();
            services.AddSingleton<IIdentityService, IdentityService>();


            services.AddScoped<LandingItemViewModel>();
            services.AddScoped<PostsViewModel>();
            services.AddScoped<PostDetailViewModel>();
            services.AddScoped<BidDialogViewModel>();
            services.AddScoped<FavoriteViewModel>();
            services.AddScoped<ProfileViewModel>();
            services.AddScoped<EditProfileViewModel>();
            services.AddScoped<ChangePasswordViewModel>();
            services.AddScoped<DiscoverViewModel>();
            services.AddScoped<ShellViewModel>();

            services.AddAutoMapper(typeof(PostProfile).Assembly);

            services.AddSingleton((s) => BlobCache.LocalMachine);
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
            BlobCache.ApplicationName = "UrGuide";
            Akavache.Registrations.Start("UrGuide");
        }

        private static void RegisterRoutes()
        {
            Routing.RegisterRoute("posts", typeof(PostPage));
            Routing.RegisterRoute("posts/details", typeof(PostDetailPage));
            Routing.RegisterRoute("postdetails", typeof(PostDetailPage));
            Routing.RegisterRoute("profile", typeof(Views.Profile));
            Routing.RegisterRoute("discover", typeof(Discover));
        }
    }
    public static class LinkerPreserve
    {
        static LinkerPreserve()
        {
            var encryptedName = typeof(SQLiteEncryptedBlobCache).FullName;
            var suName = typeof(SQLitePersistentBlobCache).FullName;
        }
    }
}
