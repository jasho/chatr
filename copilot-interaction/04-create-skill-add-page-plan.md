# Create "Add MAUI Page" Skill

## Problem Statement
Encode the repeatable steps for adding a new page to `ChatR.Maui.App` as a Copilot CLI skill, so the process can be executed consistently without re-deriving the steps each time.

## What Is a Skill
A Copilot CLI skill is a `.github/skills/<name>/SKILL.md` file with YAML frontmatter and Markdown instructions. Copilot automatically loads it when relevant, or it can be invoked explicitly with `/maui-add-page`.

## Steps Observed When Adding a Page (from tasks 03 & 04)
1. Create `ViewModels/<PageName>ViewModel.cs` — `partial class`, inherits `ViewModelBase`, overrides `LoadDataAsync()`
2. Create `Pages/<PageName>.xaml.cs` — inherits `ContentPageBase<TViewModel>`, constructor-injects VM, calls `InitializeComponent()`
3. Create `Pages/<PageName>.xaml` — root element is `inf:ContentPageBase` with `x:TypeArguments`, `x:Class` must be `ChatR.Maui.App.Pages.<PageName>` (not root namespace), MAUI xmlns present
4. Update `RoutingService.cs` — add `using ChatR.Maui.App.Pages`, add route const `<PageName>Route = "//<routename>"`, add `RouteModel` entry to the routes collection
5. Update `MauiProgram.cs` — add `Pages.<PageName>` to `ConfigureViews`, add `ViewModels.<PageName>ViewModel` to `ConfigureViewModels`
6. Update `AppShell.xaml` — add `ShellContent` with `pages:` namespace, matching `Route="<routename>"`

## Key Gotchas (learned the hard way)
- `x:Class` in XAML **must** match the code-behind namespace (`ChatR.Maui.App.Pages.*`), not the root namespace
- `RoutingService.cs` needs explicit `using ChatR.Maui.App.Pages` — type resolution won't work from `Infrastructure.Navigation` namespace without it
- `MauiProgram.cs` uses qualified names `Pages.XPage` and `ViewModels.XViewModel`
- MAUI xmlns `xmlns="http://schemas.microsoft.com/dotnet/2021/maui"` must be present in XAML

## Todos
1. **create-skill-dir** — Create `.github/skills/maui-add-page/` directory
2. **create-skill-md** — Write `SKILL.md` with full instructions
