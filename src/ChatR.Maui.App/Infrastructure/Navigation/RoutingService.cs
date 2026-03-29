using ChatR.Maui.App.Pages;

namespace ChatR.Maui.App.Infrastructure.Navigation;

public class RoutingService : IRoutingService
{
    public const string MainPageRoute = "//main";
    public const string SettingsPageRoute = "//settings";

    private static readonly IEnumerable<RouteModel> routes =
    [
        new(MainPageRoute, typeof(MainPage)),
        new(SettingsPageRoute, typeof(SettingsPage))
    ];

    public IEnumerable<RouteModel> Routes => routes;
}
