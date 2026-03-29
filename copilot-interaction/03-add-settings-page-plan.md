# Add Settings Page Plan

## Problem Statement
Add a `SettingsPage` to `ChatR.Maui.App` following the established MVVM infrastructure pattern: ViewModel injected via constructor, page inheriting `ContentPageBase<TViewModel>`, route registered in `RoutingService`, both page and VM registered in DI, and the page accessible from the Shell.

## Current State
- Infrastructure in place: `ViewModelBase`, `ContentPageBase<T>`, `RoutingService`, DI wiring in `MauiProgram.cs`
- Only `MainPage` exists, with route `//main`
- `AppShell.xaml` has a single `ShellContent` for MainPage

## Files to Create

| File | Description |
|---|---|
| `ViewModels/SettingsPageViewModel.cs` | Inherits `ViewModelBase`, overrides `LoadDataAsync()` |
| `Pages/SettingsPage.xaml.cs` | Inherits `ContentPageBase<SettingsPageViewModel>`, constructor injection |
| `Pages/SettingsPage.xaml` | XAML with base class root element, placeholder Settings UI |

## Files to Update

| File | Change |
|---|---|
| `Infrastructure/Navigation/RoutingService.cs` | Add `SettingsPageRoute = "//settings"` constant + `RouteModel` entry |
| `MauiProgram.cs` | Add `SettingsPage` to `ConfigureViews`, `SettingsPageViewModel` to `ConfigureViewModels` |
| `AppShell.xaml` | Add second `ShellContent` with `Route="settings"` pointing to `SettingsPage` |

## Todos

1. **settings-vm** — Create `SettingsPageViewModel`
2. **settings-page-cs** — Create `SettingsPage` code-behind
3. **settings-page-xaml** — Create `SettingsPage` XAML
4. **routing-service-update** — Add route to `RoutingService`
5. **di-registration** — Register in `MauiProgram.cs` DI
6. **shell-update** — Add to `AppShell.xaml`
7. **verify-build** — `dotnet build ChatR.slnx` → 0 errors
