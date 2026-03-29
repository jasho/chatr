namespace ChatR.Maui.App.Infrastructure.Navigation;

public interface IRoutingService
{
    IEnumerable<RouteModel> Routes { get; }
}
