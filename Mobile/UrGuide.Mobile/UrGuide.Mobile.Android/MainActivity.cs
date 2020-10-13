using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter.Distribute;
using System.Net;
using Xamarin.Forms;
using Sharpnado.Presentation.Forms.Droid;
using Sharpnado.MaterialFrame.Droid;
using Android.Support.V7.App;

namespace UrGuide.Mobile.Droid
{
    [Activity(Label = "UrGuide", 
        Icon = "@drawable/logo_clean",
        MainLauncher =true,
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait,
        Theme = "@style/MainTheme", 
        ConfigurationChanges = ConfigChanges.ScreenSize,
        LaunchMode = LaunchMode.SingleTop
        )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ServicePointManager
            .ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) => true;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Distribute.SetEnabledForDebuggableBuild(false);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FormsMaterial.Init(this, savedInstanceState);
            SharpnadoInitializer.Initialize(enableInternalLogger: true, enableInternalDebugLogger: true);
            AndroidMaterialFrameRenderer.ThrowStopExceptionOnDraw = false;
            AndroidMaterialFrameRenderer.BlurAutoUpdateDelayMilliseconds = 200;
            AndroidMaterialFrameRenderer.BlurProcessingDelayMilliseconds = 100;
            Forms.Init(RegisterServices);
            LoadApplication(Forms.Ioc.GetService<App>());
        }

        private void RegisterServices(IServiceCollection services)
        {
            CachedImageRenderer.Init(true);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
    //[Activity(Label = "UrGuide",
    //    Icon = "@mipmap/logo_clean",
    //    //MainLauncher = true,
    //    NoHistory = true,
    //    ScreenOrientation = ScreenOrientation.Portrait,
    //    Theme = "@style/Splash",
    //    LaunchMode = LaunchMode.SingleTop
    //    )]
    //public class SplashScreen : AppCompatActivity
    //{
    //    protected override void OnResume()
    //    {
    //        base.OnResume();
    //        RunOnUiThread(() => StartActivity(typeof(MainActivity)));
    //    }
    //}
}