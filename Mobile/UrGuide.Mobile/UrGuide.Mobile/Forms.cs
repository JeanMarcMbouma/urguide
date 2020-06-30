using Microsoft.Extensions.DependencyInjection;
using System;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Services;

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
        }
        public static void Init(Action<IServiceCollection> registerServices)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            registerServices?.Invoke(services);
            Ioc = services.BuildServiceProvider();
        }
    }
}
