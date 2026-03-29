Add support for global imports to the project.
On the C# side - add GlobalUsings.cs file and add all the namespaces that are commonly used - project root, pages, ViewModels etc.
On the XAML side - add Imports.cs file and add assembly level namespaces based on https://egvijayanand.in/2025/09/24/what-is-new-in-dotnet-maui-10-global-and-implicit-namespaces-for-xaml/#global-namespace
Update usages:
- Go through the C# files and remove unnecessary usings.
- Go through the XAML files and replace xmlns="http://schemas.microsoft.com/dotnet/2021/maui" with xmlns="http://schemas.microsoft.com/dotnet/maui/global", remove all xmlns:XXX namespaces and their usage in the XAML files. And add all these namespaces to the Imports.cs file.

Also update the maui-add-page skill to create new pages using these settings - so that the xmlns is set to the correct value and no xmlns namespaces are created.