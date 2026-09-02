using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;

namespace ChatR.Maui.App.Services;

public class ChatService : IChatService
{
    private readonly string _hubUrl;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private HubConnection? _connection;

    public event Action<ChatMessage>? MessageReceived;
    public event Action<bool>? ConnectionStateChanged;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public ChatService(IOptions<AppSettings> options)
    {
        var serverUrl = options.Value.ServerUrl.TrimEnd('/');
        _hubUrl = $"{serverUrl}{ChatHubConstants.HubPath}";
    }

    public async Task ConnectAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            if (_connection is { State: HubConnectionState.Connected or HubConnectionState.Connecting or HubConnectionState.Reconnecting })
                return;

            if (_connection is null)
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl, options =>
                    {
                        options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                        options.SkipNegotiation = false;
                    })
                    .WithAutomaticReconnect()
                    .Build();

                _connection.On<ChatMessage>(ChatHubConstants.ReceiveMessage, message =>
                    MessageReceived?.Invoke(message));

                _connection.Reconnecting += _ =>
                {
                    ConnectionStateChanged?.Invoke(false);
                    return Task.CompletedTask;
                };
                _connection.Reconnected += _ =>
                {
                    ConnectionStateChanged?.Invoke(true);
                    return Task.CompletedTask;
                };
                _connection.Closed += _ =>
                {
                    ConnectionStateChanged?.Invoke(false);
                    return Task.CompletedTask;
                };
            }

            try
            {
                await _connection.StartAsync();
                ConnectionStateChanged?.Invoke(true);
            }
            catch (OperationCanceledException)
            {
                // Expected when user navigates away quickly and connection startup is interrupted.
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task SendMessageAsync(string sender, string text)
    {
        if (_connection is null || _connection.State != HubConnectionState.Connected)
            return;

        await _connection.InvokeAsync(ChatHubConstants.SendMessage, sender, text);
    }

    public async Task DisconnectAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            if (_connection is null)
                return;

            if (_connection.State is HubConnectionState.Connected or HubConnectionState.Connecting or HubConnectionState.Reconnecting)
            {
                try
                {
                    await _connection.StopAsync();
                }
                catch (OperationCanceledException)
                {
                    // Expected when rapid page changes cancel in-flight transport operations.
                }
            }

            await _connection.DisposeAsync();
            _connection = null;
        }
        finally
        {
            _connectionLock.Release();
        }
    }
}
