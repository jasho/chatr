using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace ChatR.Maui.App.Services;

public class ChatService : IChatService
{
    private readonly string _hubUrl;
    private HubConnection? _connection;

    public event Action<ChatMessage>? MessageReceived;

    public ChatService(IOptions<AppSettings> options)
    {
        _hubUrl = options.Value.ServerUrl.TrimEnd('/') + ChatHubConstants.HubPath;
    }

    public async Task ConnectAsync()
    {
        if (_connection is { State: HubConnectionState.Connected })
            return;

        _connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<ChatMessage>(ChatHubConstants.ReceiveMessage, message =>
            MessageReceived?.Invoke(message));

        await _connection.StartAsync();
    }

    public async Task SendMessageAsync(string sender, string text)
    {
        if (_connection is null || _connection.State != HubConnectionState.Connected)
            return;

        await _connection.InvokeAsync(ChatHubConstants.SendMessage, sender, text);
    }

    public async Task DisconnectAsync()
    {
        if (_connection is not null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}
