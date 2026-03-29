# ChatR Project Scaffolding Plan

## Problem Statement
Set up a new .NET solution `ChatR.slnx` (slnx XML format) in `C:\Projects\MAUI\chatr` with three projects demonstrating chat via SignalR on .NET MAUI.

## Environment
- .NET SDK: 10.0.200
- MAUI workloads: android, ios, maui-windows (all installed)
- Repo is empty except for .git, .gitignore, LICENSE, README.md

## Projects

| Project | Template | Target | Notes |
|---|---|---|---|
| `ChatR.Common` | `classlib` | `net10.0` | Shared models/contracts |
| `ChatR.Maui.App` | `maui` | `net10.0-*` | Mobile/desktop UI |
| `ChatR.Server.App` | `webapi` | `net10.0` | ASP.NET Core + SignalR |

## Approach

The `.slnx` format is an XML-based solution format. Since `dotnet new slnx` may not be available as a standalone template in this SDK version, create the file manually — it's a simple XML structure. Then use `dotnet sln` to add projects (which supports slnx in .NET 10) or add project entries directly to the XML.

## Todos

1. **scaffold-common** — Create `ChatR.Common` class library (net10.0)
2. **scaffold-maui** — Create `ChatR.Maui.App` MAUI app (net10.0-android/ios/windows)
3. **scaffold-server** — Create `ChatR.Server.App` ASP.NET Core Web API (net10.0) with SignalR referenced
4. **create-slnx** — Create `ChatR.slnx` solution file and add all three projects to it
5. **verify** — Build the solution to confirm everything compiles

## Notes
- `ChatR.Common` uses the plain `classlib` template (not `mauilib`) since it targets `net10.0` only
- `ChatR.Server.App` uses `webapi` template; SignalR is built into `Microsoft.AspNetCore.App` so no extra NuGet needed — just wire up `AddSignalR()` / `MapHub<>()` as minimal scaffolding
- For slnx creation: if `dotnet new slnx` works, use it; otherwise write the XML manually and use `dotnet sln add` to register projects
- All projects go in subdirectories of the repo root
