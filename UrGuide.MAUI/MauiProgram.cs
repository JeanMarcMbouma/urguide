using Microsoft.Extensions.Logging;
using UrGuide.MAUI.Services;
using UrGuide.MAUI.ViewModels;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Akavache;
using IdentityModel.OidcClient.Browser;
using UrGuide.MAUI.Services.Identity;
using UrGuide.MAUI.Mapping;
using CommunityToolkit.Maui;

namespace UrGuide.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Bold.ttf", "FontBold");
                fonts.AddFont("OpenSans-SemiBold.ttf", "FontSemiBold");
                fonts.AddFont("OpenSans-ExtraBold.ttf", "FontExtraBold");
                fonts.AddFont("OpenSans-Light.ttf", "FontLight");
                fonts.AddFont("Font Awesome 5 Free-Solid-900.otf", "fas");
                fonts.AddFont("Font Awesome 5 Free-Regular-400.otf", "far");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        ConfigureServices(builder.Services);
        
        var app = builder.Build();
        
        InitializeApplication();
        
        return app;
    }

    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IPostItemService, PostItemService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IPreferenceService, PreferenceService>();
        services.AddSingleton<IIdentityService, IdentityService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IdentityModel.OidcClient.Browser.IBrowser, SystemBrowser>();

        // ViewModels
        services.AddScoped<PostsViewModel>();
        services.AddScoped<PostDetailViewModel>();
        services.AddScoped<BidDialogViewModel>();
        services.AddScoped<FavoriteViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddScoped<EditProfileViewModel>();
        services.AddScoped<ChangePasswordViewModel>();
        services.AddScoped<DiscoverViewModel>();
        services.AddScoped<ShellViewModel>();
        services.AddScoped<MainPageViewModel>();
        services.AddTransient<CreatePostViewModel>();

        // Views
        services.AddTransient<Views.DiscoverPage>();
        services.AddTransient<Views.PostsPage>();
        services.AddTransient<Views.FavoritePage>();
        services.AddTransient<Views.ProfilePage>();

        // AutoMapper
        services.AddAutoMapper(typeof(PostProfile).Assembly);

        // Akavache
        services.AddSingleton((s) => BlobCache.LocalMachine);

        return services;
    }

    public static void InitializeApplication()
    {
        // Initialize Akavache
        BlobCache.ApplicationName = "UrGuide";
    }
}