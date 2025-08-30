using Microsoft.Extensions.Logging;
using UrGuide.MAUI.Services;
using UrGuide.MAUI.ViewModels;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Akavache;
using IdentityModel.OidcClient.Browser;
using UrGuide.MAUI.Services.Identity;
using UrGuide.MAUI.Mapping;

namespace UrGuide.MAUI;

public static class ServiceConfiguration
{
    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IPostItemService, PostItemService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IPreferenceService, PreferenceService>();
        services.AddSingleton<IIdentityService, IdentityService>();
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IBrowser, SystemBrowser>();

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