namespace ChatR.Maui.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        ConfigureAppSettings(builder);
        ConfigureViews(builder.Services);
        ConfigureViewModels(builder.Services);
        ConfigureServices(builder.Services, builder.Configuration);

#if DEBUG
        builder.Logging.AddDebug();
        builder.AddMauiDevFlowAgent();
#endif

        var app = builder.Build();
        RegisterRoutes(app);
        return app;
    }

    private static void ConfigureAppSettings(MauiAppBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var configBuilder = new ConfigurationBuilder();

        var appSettingsResource = "ChatR.Maui.App.Configuration.appsettings.json";
        using var appSettingsStream = assembly.GetManifestResourceStream(appSettingsResource);
        if (appSettingsStream is not null)
            configBuilder.AddJsonStream(appSettingsStream);

        var devSettingsResource = "ChatR.Maui.App.Configuration.appsettings.Development.json";
        using var devSettingsStream = assembly.GetManifestResourceStream(devSettingsResource);
        if (devSettingsStream is not null)
            configBuilder.AddJsonStream(devSettingsStream);

        builder.Configuration.AddConfiguration(configBuilder.Build());
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddTransient<MainPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<ChatPage>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<SettingsPageViewModel>();
        services.AddTransient<ChatPageViewModel>();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRoutingService, RoutingService>();
        services.Configure<AppSettings>(options =>
            configuration.GetSection("AppSettings").Bind(options));
        services.AddSingleton<IChatService, ChatService>();
    }

    private static void RegisterRoutes(MauiApp app)
    {
        var routingService = app.Services.GetRequiredService<IRoutingService>();
        foreach (var route in routingService.Routes)
            Routing.RegisterRoute(route.Route, route.ViewType);
    }
}
