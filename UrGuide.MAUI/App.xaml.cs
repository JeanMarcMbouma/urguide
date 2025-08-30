using UrGuide.MAUI.Views;

namespace UrGuide.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}