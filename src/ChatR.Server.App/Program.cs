using ChatR.Common;
using ChatR.Server.App;
using ChatR.Server.App.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OllamaSharp;
using AiMessage = Microsoft.Extensions.AI.ChatMessage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials());
});

builder.Services.Configure<OllamaSettings>(builder.Configuration.GetSection("OllamaSettings"));
builder.Services.AddSingleton<IChatClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;
    return new OllamaApiClient(new Uri(settings.Endpoint), settings.Model);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseCors();

app.MapHub<ChatHub>(ChatHubConstants.HubPath);

app.MapPost(AiChatConstants.RoutePath, async (AiChatRequest request, IChatClient chatClient, CancellationToken cancellationToken) =>
{
    var messages = request.Messages
        .Select(turn => new AiMessage(new ChatRole(turn.Role), turn.Content))
        .ToList();

    var response = await chatClient.GetResponseAsync(messages, cancellationToken: cancellationToken);
    return Results.Ok(new AiChatResponse(response.Text));
});

app.Run();
