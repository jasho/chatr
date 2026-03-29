# Add Global Imports Plan

## Problem Statement
Reduce boilerplate by introducing C# global usings and XAML global namespace mappings. After this, new pages need no per-file `using` statements and no per-file XAML namespace prefix declarations.

## Reference
XAML global namespace feature: https://egvijayanand.in/2025/09/24/what-is-new-in-dotnet-maui-10-global-and-implicit-namespaces-for-xaml/#global-namespace

## Current State

**Usings in use across C# files:**
- `ChatR.Maui.App.Infrastructure.Navigation`
- `ChatR.Maui.App.Infrastructure.Pages`
- `ChatR.Maui.App.Infrastructure.ViewModels`
- `ChatR.Maui.App.Pages`
- `ChatR.Maui.App.ViewModels`
- `CommunityToolkit.Maui`
- `CommunityToolkit.Mvvm.ComponentModel`
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.Logging`
- `System.Reflection`

**XAML xmlns prefixes in use:**
- `xmlns:inf="clr-namespace:ChatR.Maui.App.Infrastructure.Pages"` (page base class)
- `xmlns:vm="clr-namespace:ChatR.Maui.App.ViewModels"` (VM type arguments)
- `xmlns:pages="clr-namespace:ChatR.Maui.App.Pages"` (DataTemplate in AppShell)
- `xmlns:local="clr-namespace:ChatR.Maui.App"` (unused after Settings/Chat pages)

## New Files

### `GlobalUsings.cs`
```csharp
global using ChatR.Maui.App;
global using ChatR.Maui.App.Infrastructure.Navigation;
global using ChatR.Maui.App.Infrastructure.Pages;
global using ChatR.Maui.App.Infrastructure.ViewModels;
global using ChatR.Maui.App.Pages;
global using ChatR.Maui.App.ViewModels;
global using CommunityToolkit.Maui;
global using CommunityToolkit.Mvvm.ComponentModel;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using System.Reflection;
```

### `Imports.cs`
```csharp
using Microsoft.Maui.Controls;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "ChatR.Maui.App")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "ChatR.Maui.App.Pages")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "ChatR.Maui.App.ViewModels")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "ChatR.Maui.App.Infrastructure.Pages")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "ChatR.Maui.App.Infrastructure.Navigation")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "http://schemas.microsoft.com/dotnet/2022/maui/toolkit")]
```

## XAML Before → After (page example)

**Before:**
```xml
<inf:ContentPageBase
    x:TypeArguments="vm:ChatPageViewModel"
    x:Class="ChatR.Maui.App.Pages.ChatPage"
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:inf="clr-namespace:ChatR.Maui.App.Infrastructure.Pages"
    xmlns:vm="clr-namespace:ChatR.Maui.App.ViewModels"
    Title="Chat">
```

**After:**
```xml
<ContentPageBase
    x:TypeArguments="ChatPageViewModel"
    x:Class="ChatR.Maui.App.Pages.ChatPage"
    xmlns="http://schemas.microsoft.com/dotnet/maui/global"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    Title="Chat">
```

## Todos

1. **global-usings** — Create `GlobalUsings.cs`
2. **imports-cs** — Create `Imports.cs` with `XmlnsDefinition` attributes
3. **clean-cs-usings** — Remove redundant `using` from all C# files
4. **update-xaml** — Update all XAML files to global namespace
5. **update-skill** — Update `maui-add-page` skill with new XAML template
6. **verify-build** — `dotnet build ChatR.slnx` → 0 errors
