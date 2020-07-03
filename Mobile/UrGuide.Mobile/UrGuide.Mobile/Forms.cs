using Microsoft.Extensions.DependencyInjection;
using System;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.ViewModels;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

namespace UrGuide.Mobile
{
    public static class Forms
    {
        public static IServiceProvider Ioc { get; private set; }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<AppShell>();
            services.AddSingleton<IMainPageService, MainPageService>();
            services.AddSingleton<App>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddScoped<LandingItemViewModel>();
            services.AddScoped<PostsViewModel>();
        }
        public static void Init(Action<IServiceCollection> registerServices)
        {
            Device.SetFlags(new[] { 
                "CollectionView_Experimental", 
                "Shapes_Experimental", 
                "CarouselView_Experimental",
                "Expander_Experimental",
                //"FastRenderers_Experimental"
            });

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
        }
    }
}
