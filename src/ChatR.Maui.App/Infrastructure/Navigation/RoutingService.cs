namespace ChatR.Maui.App.Infrastructure.Navigation;

public class RoutingService : IRoutingService
{
    public const string SettingsPageRoute = "//settings";
    public const string ChatPageRoute = "//chat";
    public const string AiChatPageRoute = "//aichat";

    private static readonly IEnumerable<RouteModel> routes =
    [
        new(SettingsPageRoute, typeof(SettingsPage)),
        new(ChatPageRoute, typeof(ChatPage)),
        new(AiChatPageRoute, typeof(AiChatPage))
    ];

    public IEnumerable<RouteModel> Routes => routes;
}
