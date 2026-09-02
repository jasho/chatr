namespace ChatR.Maui.App.Services;

public interface IAiChatService
{
    bool HasApiKey { get; }

    /// <summary>Sends the user's message to the AI model and returns its reply.</summary>
    Task<string> SendMessageAsync(string userMessage, CancellationToken cancellationToken = default);

    /// <summary>Clears the in-memory conversation history.</summary>
    void ResetConversation();
}
