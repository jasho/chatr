namespace ChatR.Maui.App.Infrastructure.Navigation;

public class RoutingService : IRoutingService
{
    public const string MainPageRoute = "//main";

    private static readonly IEnumerable<RouteModel> routes =
    [
        new(MainPageRoute, typeof(MainPage))
    ];

    public IEnumerable<RouteModel> Routes => routes;
}
