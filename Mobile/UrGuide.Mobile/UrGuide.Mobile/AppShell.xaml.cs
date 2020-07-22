using Plugin.SharedTransitions;
using System.Linq;
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
