---
name: maui-add-page
description: Use this skill when asked to add a new page to the ChatR.Maui.App project. It covers creating the ViewModel, the Page (XAML + code-behind), and wiring everything into the RoutingService, DI container, and AppShell.
---

# Adding a New Page to ChatR.Maui.App

Follow these steps **in order** when adding a new page. Replace `<PageName>` with the PascalCase name of the page (e.g. `Chat`, `Profile`) and `<routename>` with a lowercase route slug (e.g. `chat`, `profile`).

---

## Step 1 — Create the ViewModel

Create `src/ChatR.Maui.App/ViewModels/<PageName>ViewModel.cs`:

```csharp
namespace ChatR.Maui.App.ViewModels;

public partial class <PageName>ViewModel : ViewModelBase
{
    protected override Task LoadDataAsync()
        => Task.CompletedTask;
}
```

---

## Step 2 — Create the Page code-behind

Create `src/ChatR.Maui.App/Pages/<PageName>.xaml.cs`:

```csharp
namespace ChatR.Maui.App.Pages;

public partial class <PageName> : ContentPageBase<<PageName>ViewModel>
{
    public <PageName>(<PageName>ViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
```

---

## Step 3 — Create the Page XAML

Create `src/ChatR.Maui.App/Pages/<PageName>.xaml`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPageBase
    x:TypeArguments="<PageName>ViewModel"
    x:Class="ChatR.Maui.App.Pages.<PageName>"
    xmlns="http://schemas.microsoft.com/dotnet/maui/global"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    Title="<PageName>">

    <VerticalStackLayout
        Padding="30"
        Spacing="16"
        VerticalOptions="Center">
        <Label
            Text="<PageName>"
            HorizontalOptions="Center"
            FontSize="24" />
    </VerticalStackLayout>

</ContentPageBase>
```

> **CRITICAL:** `x:Class` must be `ChatR.Maui.App.Pages.<PageName>` — matching the code-behind's namespace exactly. Do NOT use the root namespace `ChatR.Maui.App.<PageName>` or the XAML source generator will produce a broken parameterless constructor call.

---

## Step 4 — Register the route in RoutingService

Edit `src/ChatR.Maui.App/Infrastructure/Navigation/RoutingService.cs`:

1. Add a route constant: `public const string <PageName>Route = "//<routename>";`
2. Add a `RouteModel` entry to the `routes` collection: `new(<PageName>Route, typeof(<PageName>))`

> **Note:** No `using` statement is needed — all project namespaces are globally imported via `GlobalUsings.cs`.

Example after adding a `ProfilePage` with route `//profile`:

```csharp
namespace ChatR.Maui.App.Infrastructure.Navigation;

public class RoutingService : IRoutingService
{
    public const string MainPageRoute = "//main";
    public const string SettingsPageRoute = "//settings";
    public const string ChatPageRoute = "//chat";
    public const string ProfilePageRoute = "//profile";

    private static readonly IEnumerable<RouteModel> routes =
    [
        new(MainPageRoute, typeof(MainPage)),
        new(SettingsPageRoute, typeof(SettingsPage)),
        new(ChatPageRoute, typeof(ChatPage)),
        new(ProfilePageRoute, typeof(ProfilePage))
    ];

    public IEnumerable<RouteModel> Routes => routes;
}
```

---

## Step 5 — Register in the DI container

Edit `src/ChatR.Maui.App/MauiProgram.cs`:

- In `ConfigureViews`: add `services.AddTransient<<PageName>>();`
- In `ConfigureViewModels`: add `services.AddTransient<<PageName>ViewModel>();`

---

## Step 6 — Add to AppShell

Edit `src/ChatR.Maui.App/AppShell.xaml`, add a `<ShellContent>` entry:

```xml
<ShellContent
    Title="<PageName>"
    ContentTemplate="{DataTemplate <PageName>}"
    Route="<routename>" />
```

> **Note:** The `Route` value is just the slug (e.g. `profile`), not the full path (e.g. `//profile`). The `//` prefix is only used in `RoutingService` constants and programmatic navigation. No `xmlns:pages` prefix is needed — types are resolved via the global XAML namespace.

---

## Verification

After all steps, build to confirm no errors:

```shell
cd src && dotnet build ChatR.slnx
```

Expected output: `Build succeeded. 0 Warning(s) 0 Error(s)`
