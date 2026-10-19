using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Platform;
using Microsoft.Maui.ApplicationModel;

namespace BiztvillCRM.Mobile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override void OnBackPressed()
    {
        var blazorWebView = (Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page as MainPage)?.BlazorWebView;
        var androidWebView = blazorWebView?.Handler?.PlatformView as Android.Webkit.WebView;

        if (androidWebView is not null && androidWebView.CanGoBack())
        {
            androidWebView.GoBack();
            return;
        }

        base.OnBackPressed();
    }
}
