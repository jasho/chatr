namespace ChatR.Maui.App.Services;

public interface IChatService
{
    event Action<ChatMessage> MessageReceived;
    event Action<bool> ConnectionStateChanged;

    bool IsConnected { get; }

    Task ConnectAsync();
    Task SendMessageAsync(string sender, string text);
    Task DisconnectAsync();
}
