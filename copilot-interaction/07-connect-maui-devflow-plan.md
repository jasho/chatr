# Connect MAUI DevFlow Plan

## What
Add `Redth.MauiDevFlow.Agent` (v0.23.1) to ChatR.Maui.App so the Copilot CLI can inspect the running app on an Android device via the maui-devflow MCP tools.

## Package Details
- NuGet: `Redth.MauiDevFlow.Agent` by Redth
- Debug-only — must NOT ship in Release builds
- Broker default port: 19223

## Code Changes Required

### ChatR.Maui.App.csproj
Add a Debug-only ItemGroup:
```xml
<ItemGroup Condition="'$(Configuration)'=='Debug'">
  <PackageReference Include="Redth.MauiDevFlow.Agent" Version="0.23.1" />
</ItemGroup>
```

### MauiProgram.cs
Add inside CreateMauiApp:
```csharp
#if DEBUG
builder.AddMauiDevFlowAgent();
#endif
```
Also needs: `using MauiDevFlow.Agent;` (via conditional global using or inline)

## Android Device Setup (one-time, after app starts)
```sh
adb reverse tcp:19223 tcp:19223
```
This tunnels the broker port so the CLI can discover and talk to the agent inside the app.

## Verification
1. `maui_wait` — block until agent connects
2. `maui_screenshot` — take screenshot proving connected to running app

## Todos
1. add-devflow-pkg — add NuGet package (Debug-only), run dotnet restore
2. configure-devflow — add `builder.AddMauiDevFlowAgent()` under #if DEBUG
3. build-deploy-android — build & deploy to connected Android device  
4. adb-broker-forward — run `adb reverse tcp:19223 tcp:19223`
5. verify-screenshot — maui_wait + maui_screenshot
