# Fix 09: Virtual Keyboard Overlapping Entry — Plan

## Problem
When the user taps the `Entry` field on Android, the soft keyboard opens and **covers the compose bar** (Entry + Send button). The user cannot see what they are typing and cannot tap Send.

## Root Cause
`SoftInput.AdjustResize` was set in fix 08 but is **non-functional on Android API 30+** (Android 11+; the test device runs Android 14 = API 34). The reason: MAUI .NET 10 enables **edge-to-edge rendering** by default via `WindowCompat.setDecorFitsSystemWindows(window, false)` inside `MauiAppCompatActivity.OnCreate`. Once edge-to-edge is active, the system completely ignores all `windowSoftInputMode` flags including `AdjustResize`.

## Desired Behaviour
- Keyboard opens → window content area **shrinks** to fit above the keyboard
- Compose bar (Entry + Button) stays **visible at the bottom** of the available space
- `CollectionView` (Row 0 `*`) fills the remaining space — no content pushed off-screen
- The existing `Grid` layout (`RowDefinitions="*,Auto"`) is already correct; only Android window insets need fixing

## Fix

### `src/ChatR.Maui.App/Platforms/Android/MainActivity.cs`
After `base.OnCreate`, call `WindowCompat.SetDecorFitsSystemWindows(Window!, true)` to disable edge-to-edge, restoring traditional inset handling which allows `AdjustResize` to resize the app window when the keyboard appears.

```csharp
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace ChatR.Maui.App;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        WindowCompat.SetDecorFitsSystemWindows(Window!, true);
        Window?.SetSoftInputMode(SoftInput.AdjustResize);
    }
}
```

`AndroidX.Core.View.WindowCompat` is available as a MAUI transitive dependency — no extra NuGet needed.

## Todos
1. Update `MainActivity.cs` with `WindowCompat.SetDecorFitsSystemWindows`
2. Build and deploy: `dotnet build -f net10.0-android -t:Run`
3. Verify: tap Entry via `adb shell input tap`, screenshot, confirm compose bar visible above keyboard
