using Plugin.SharedTransitions;
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
    }
}
