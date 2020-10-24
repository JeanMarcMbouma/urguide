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
using IdentityModel.OidcClient.Browser;

[assembly: ExportFont("Font Awesome 5 Free-Solid-900.otf", Alias = "fas")]
[assembly: ExportFont("Font Awesome 5 Free-Regular-400.otf", Alias = "far")]
[assembly: ExportFont("OpenSansBold.ttf", Alias = "FontBold")]
[assembly: ExportFont("OpenSans-ExtraBold.ttf", Alias = "FontExtraBold")]
[assembly: ExportFont("OpenSans-BoldItalic.ttf", Alias = "FontBoldItalic")]
[assembly: ExportFont("OpenSans-Italic.ttf", Alias = "FontItalic")]
[assembly: ExportFont("OpenSans-Light.ttf", Alias = "FontLight")]
[assembly: ExportFont("OpenSans-Regular.ttf", Alias = "FontRegular")]
[assembly: ExportFont("OpenSans-SemiBold.ttf", Alias = "FontSemiBold")]

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
            services.AddHttpClient<API.UsersClient>(clientRegistration);
            services.AddHttpClient<API.CatalogsClient>(clientRegistration);

            services.AddSingleton<IMainPageService, NavigationPageService>();
            services.AddSingleton<App>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IPostItemService, PostItemService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IPreferenceService, PreferenceService>();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IBrowser, SystemBrowser>();

            services.AddScoped<PostsViewModel>();
            services.AddScoped<PostDetailViewModel>();
            services.AddScoped<BidDialogViewModel>();
            services.AddScoped<FavoriteViewModel>();
            services.AddScoped<ProfileViewModel>();
            services.AddScoped<EditProfileViewModel>();
            services.AddScoped<ChangePasswordViewModel>();
            services.AddScoped<DiscoverViewModel>();
            services.AddScoped<ShellViewModel>();
            services.AddScoped<MainPageViewModel>();
            services.AddTransient<CreatePostViewModel>();

            services.AddAutoMapper(typeof(PostProfile).Assembly);

            services.AddSingleton((s) => BlobCache.LocalMachine);
            services.AddSingleton((s) => MessagingCenter.Instance);
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

#if !DEBUG

            AppCenter.Start("android=c61793ee-ed79-4a3d-b023-15303f271bb4;" /*+
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here}"*/,
                   typeof(Crashes), typeof(Analytics), typeof(Distribute));
#endif

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
