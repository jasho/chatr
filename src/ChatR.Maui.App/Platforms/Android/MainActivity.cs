using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.View;

namespace ChatR.Maui.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ViewCompat.SetOnApplyWindowInsetsListener(Window!.DecorView, new KeyboardInsetsListener());
    }

    private class KeyboardInsetsListener : Java.Lang.Object, IOnApplyWindowInsetsListener
    {
        public WindowInsetsCompat OnApplyWindowInsets(Android.Views.View v, WindowInsetsCompat insets)
        {
            var imeInsets = insets.GetInsets(WindowInsetsCompat.Type.Ime());
            var navBarInsets = insets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
            var keyboardPadding = Math.Max(0, imeInsets.Bottom - navBarInsets.Bottom);

            v.FindViewById(Android.Resource.Id.Content)?.SetPadding(0, 0, 0, keyboardPadding);

            return ViewCompat.OnApplyWindowInsets(v, insets);
        }
    }
}
