namespace ChatR.Maui.App.Infrastructure.Navigation;

public class RoutingService : IRoutingService
{
    public const string MainPageRoute = "//main";
    public const string SettingsPageRoute = "//settings";
    public const string ChatPageRoute = "//chat";

    private static readonly IEnumerable<RouteModel> routes =
    [
        new(MainPageRoute, typeof(MainPage)),
        new(SettingsPageRoute, typeof(SettingsPage)),
        new(ChatPageRoute, typeof(ChatPage))
    ];

    public IEnumerable<RouteModel> Routes => routes;
}
