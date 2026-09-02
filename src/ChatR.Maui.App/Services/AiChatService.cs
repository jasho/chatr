using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using System.ClientModel;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;
using AiChatRole = Microsoft.Extensions.AI.ChatRole;

namespace ChatR.Maui.App.Services;

public class AiChatService : IAiChatService
{
    private const string SystemPrompt = "You are a helpful assistant inside the ChatR mobile app.";

    private readonly AiChatSettings _settings;
    private readonly Lazy<Microsoft.Extensions.AI.IChatClient?> _chatClient;
    private readonly List<AiChatMessage> _history = [];

    public bool HasApiKey => !string.IsNullOrWhiteSpace(_settings.ApiKey);

    public AiChatService(IOptions<AiChatSettings> options)
    {
        _settings = options.Value;
        _chatClient = new Lazy<Microsoft.Extensions.AI.IChatClient?>(CreateChatClient);
        ResetConversation();
    }

    public void ResetConversation()
    {
        _history.Clear();
        _history.Add(new AiChatMessage(AiChatRole.System, SystemPrompt));
    }

    public async Task<string> SendMessageAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        var client = _chatClient.Value
            ?? throw new InvalidOperationException(
                "No OpenRouter API key configured. Set AiChatSettings:ApiKey in appsettings.Development.json.");

        _history.Add(new AiChatMessage(AiChatRole.User, userMessage));

        var response = await client.GetResponseAsync(_history, cancellationToken: cancellationToken);
        var replyText = response.Text;

        _history.Add(new AiChatMessage(AiChatRole.Assistant, replyText));
        return replyText;
    }

    private Microsoft.Extensions.AI.IChatClient? CreateChatClient()
    {
        if (!HasApiKey)
            return null;

        var options = new OpenAIClientOptions { Endpoint = new Uri(_settings.Endpoint) };
        var openAiClient = new OpenAIClient(new ApiKeyCredential(_settings.ApiKey), options);
        return openAiClient.GetChatClient(_settings.Model).AsIChatClient();
    }
}

