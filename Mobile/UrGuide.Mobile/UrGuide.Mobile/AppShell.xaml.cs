using Microsoft.Extensions.DependencyInjection;
using Plugin.SharedTransitions;
using System.Linq;
using UrGuide.Mobile.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace UrGuide.Mobile
{
    public partial class AppShell : SharedTransitionShell
    {
        public AppShell()
        {
            InitializeComponent();
            if (!VersionTracking.IsFirstLaunchForCurrentBuild)
                RootTab.Items.RemoveAt(0);
            BindingContext = Forms.Ioc.GetRequiredService<ShellViewModel>();
        }

        public static void RemoveWelcomeScreen()
        {
            var shell = (AppShell)Shell.Current;
            var tab = shell.RootTab.Items.First();
            if (!tab.IsEnabled)
                shell.RootTab.Items.Remove(tab);
        }
    }
}
