using Microsoft.Extensions.Logging;

namespace UrGuide.MAUI.Platforms.Android;

[Android.App.Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, Android.Runtime.JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}