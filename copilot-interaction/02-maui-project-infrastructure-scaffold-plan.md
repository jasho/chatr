# ChatR.Maui.App — Infrastructure Scaffold Plan

## Problem Statement
Add MVVM infrastructure to `ChatR.Maui.App` following patterns from [cookbook-maui](https://github.com/jasho/cookbook-maui). The app currently has no architecture — just the default MAUI template. This plan scaffolds the foundational layers so all future pages follow a consistent pattern.

## Reference
- Repo: https://github.com/jasho/cookbook-maui
- Key patterns adopted: ViewModelBase, ContentPageBase<TViewModel>, RoutingService, embedded appsettings, CommunityToolkit.Maui

## Architecture Overview

```
ChatR.Maui.App/
├── Configuration/
│   ├── appsettings.json               ← embedded resource
│   └── appsettings.Development.json   ← embedded resource (optional, gitignored)
├── Infrastructure/
│   ├── Navigation/
│   │   ├── RouteModel.cs              ← record(Route, ViewType)
│   │   ├── IRoutingService.cs         ← interface
│   │   └── RoutingService.cs          ← registers all Shell routes
│   ├── Pages/
│   │   └── ContentPageBase.cs         ← abstract generic base page
│   └── ViewModels/
│       └── ViewModelBase.cs           ← abstract base with OnAppearing/LoadData
├── ViewModels/
│   └── MainPageViewModel.cs           ← first ViewModel, inherits ViewModelBase
├── MainPage.xaml / .xaml.cs           ← updated to use ContentPageBase<MainPageViewModel>
└── MauiProgram.cs                     ← updated with all wiring
```

## NuGet Packages to Add

| Package | Purpose |
|---|---|
| `CommunityToolkit.Maui` | MAUI controls + `UseMauiCommunityToolkit()` |
| `CommunityToolkit.Mvvm` | `ObservableObject`, `[ObservableProperty]`, `[RelayCommand]` |
| `Microsoft.Extensions.Configuration.Json` | JSON config stream loading |
| `Microsoft.Extensions.Configuration.Binder` | `GetSection().Bind()` options pattern |

## Key Design Decisions

### ViewModelBase
- Inherits `ObservableObject` (CommunityToolkit.Mvvm) — as specified (not `ObservableRecipient`)
- `abstract partial class` to support `[ObservableProperty]` source generation
- `[ObservableProperty] bool isBusy` — for loading indicators
- `protected bool ForceDataRefresh = true` — ensures data loads on first appear; subclasses can set to false to skip reload
- `public async Task OnAppearingAsync()` — checks `ForceDataRefresh`, calls `LoadDataAsync()`
- `protected virtual Task LoadDataAsync()` — override in concrete VMs

### ContentPageBase
- `abstract partial class ContentPageBase<TViewModel> : ContentPage where TViewModel : ViewModelBase`
- Constructor: `(TViewModel viewModel)` → sets `BindingContext = ViewModel = viewModel`
- `protected TViewModel ViewModel { get; }` — strongly typed access
- `override OnAppearing()` → calls `ViewModel.OnAppearingAsync()` (fire-and-forget with `async void`)
- No global exception service (not in scope for this task — can be added later)

### RoutingService
- `IRoutingService` interface with `IEnumerable<RouteModel> Routes { get; }`
- `RouteModel` — `record(string Route, Type ViewType)`
- `RoutingService` — registers `MainPage` route; all future pages added here
- Route constants as `public const string` fields (e.g., `MainPageRoute = "//main"`)
- Registered as `Singleton` in DI

### appsettings loading
- Files live in `Configuration/` folder, set as `<EmbeddedResource>`
- Load `appsettings.json` always
- Load `appsettings.Development.json` only if it exists as a manifest resource (handled via null check on stream)
- Uses `Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)`
- `appsettings.Development.json` added to `.gitignore`

### MauiProgram.cs structure
```csharp
builder.UseMauiApp<App>()
       .UseMauiCommunityToolkit()
       .ConfigureFonts(...)

ConfigureAppSettings(builder);   // loads embedded JSON configs
ConfigureViews(builder.Services);
ConfigureViewModels(builder.Services);
ConfigureServices(builder.Services);

var app = builder.Build();
RegisterRoutes(app);
return app;
```

## Todos

1. **nuget-packages** — Add NuGet packages + EmbeddedResource entries to csproj
2. **appsettings** — Create `Configuration/appsettings.json` and `appsettings.Development.json`
3. **viewmodel-base** — Create `Infrastructure/ViewModels/ViewModelBase.cs`
4. **contentpage-base** — Create `Infrastructure/Pages/ContentPageBase.cs`
5. **routing-service** — Create `RouteModel`, `IRoutingService`, `RoutingService`
6. **main-page-vm** — Create `ViewModels/MainPageViewModel.cs`
7. **main-page-update** — Update `MainPage.xaml` + `.xaml.cs` to use new base
8. **maui-program** — Rewrite `MauiProgram.cs` with all wiring
9. **verify-build** — `dotnet build ChatR.slnx` → 0 errors


