namespace ChatR.Maui.App.Services;

public interface IAiChatService
{
    string Provider { get; set; }
    IReadOnlyList<AiChatProviderOption> AvailableProviders { get; }

    bool IsAvailable { get; }
    string UnavailableReason { get; }

    /// <summary>Sends the user's message to the AI model and returns its reply.</summary>
    Task<string> SendMessageAsync(string userMessage, CancellationToken cancellationToken = default);

    /// <summary>Clears the in-memory conversation history.</summary>
    void ResetConversation();

    /// <summary>
    /// Produces a one-off reply for a group chat, using the given recent transcript as context.
    /// Does not touch the standalone AI Chat page's conversation history. Uses whichever
    /// <see cref="Provider"/> is currently selected at the time of the call.
    /// </summary>
    Task<string> GetContextualReplyAsync(IReadOnlyList<ChatMessage> recentMessages, CancellationToken cancellationToken = default);
}
