Create a plan to set up a project - it will be a mobile app using .NET MAUI and demonstrating chat functionality using SignalR. First of let's just create the scaffolding of the necessary projects.
Set up a .NET solution called ChatR.slnx (use the new slnx format) with 3 projects:
- ChatR.Common (common library, target .NET 10)
- ChatR.Maui.App (empty .NET MAUI App, target .NET 10, latest MAUI version)
- ChatR.Server.App (ASP.NET app, using SignalR, target .NET 10, so far only an empty ASP.NET project)