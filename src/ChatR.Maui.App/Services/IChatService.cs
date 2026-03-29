namespace ChatR.Maui.App.Services;

public interface IChatService
{
    event Action<ChatMessage> MessageReceived;

    Task ConnectAsync();
    Task SendMessageAsync(string sender, string text);
    Task DisconnectAsync();
}
