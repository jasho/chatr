using System.ClientModel;
using System.Net.Http.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;
using AiChatRole = Microsoft.Extensions.AI.ChatRole;

namespace ChatR.Maui.App.Services;

public class AiChatService : IAiChatService
{
    private const string OllamaServerProvider = "OllamaServer";
    private const string OpenRouterProvider = "OpenRouter";
    private const string SystemPrompt = "You are a helpful assistant inside the ChatR mobile app.";

    private readonly AiChatSettings _settings;
    private readonly AppSettings _appSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly Lazy<Microsoft.Extensions.AI.IChatClient?> _openRouterClient;

    private readonly List<AiChatMessage> _openRouterHistory = [];
    private readonly List<AiChatTurn> _serverHistory = [];

    private string _provider;

    public IReadOnlyList<AiChatProviderOption> AvailableProviders { get; } =
    [
        new(OpenRouterProvider, "OpenRouter (cloud)"),
        new(OllamaServerProvider, "Ollama (via ChatR Server)")
    ];

    public AiChatService(IOptions<AiChatSettings> aiChatOptions, IOptions<AppSettings> appOptions, IHttpClientFactory httpClientFactory)
    {
        _settings = aiChatOptions.Value;
        _appSettings = appOptions.Value;
        _httpClientFactory = httpClientFactory;
        _openRouterClient = new Lazy<Microsoft.Extensions.AI.IChatClient?>(CreateOpenRouterClient);

        var savedProvider = Preferences.Default.Get(PreferencesService.AiChatProviderPreferenceKey, string.Empty);
        _provider = string.IsNullOrWhiteSpace(savedProvider) ? _settings.Provider : savedProvider;

        ResetConversation();
    }

    public string Provider
    {
        get => _provider;
        set
        {
            if (string.Equals(_provider, value, StringComparison.OrdinalIgnoreCase))
                return;

            _provider = value;
            Preferences.Default.Set(PreferencesService.AiChatProviderPreferenceKey, value);
        }
    }

    private bool UsesOllamaServer => string.Equals(_provider, OllamaServerProvider, StringComparison.OrdinalIgnoreCase);

    public bool IsAvailable => UsesOllamaServer
        ? !string.IsNullOrWhiteSpace(_appSettings.ServerUrl)
        : !string.IsNullOrWhiteSpace(_settings.ApiKey);

    public string UnavailableReason => UsesOllamaServer
        ? "Set AppSettings:ServerUrl in appsettings.Development.json to point at your running ChatR.Server.App instance."
        : "Create a free key at openrouter.ai/keys and set AiChatSettings:ApiKey in appsettings.Development.json, then restart the app.";

    public void ResetConversation()
    {
        _openRouterHistory.Clear();
        _openRouterHistory.Add(new AiChatMessage(AiChatRole.System, SystemPrompt));

        _serverHistory.Clear();
        _serverHistory.Add(new AiChatTurn("system", SystemPrompt));
    }

    public Task<string> SendMessageAsync(string userMessage, CancellationToken cancellationToken = default)
        => UsesOllamaServer
            ? SendViaOllamaServerAsync(userMessage, cancellationToken)
            : SendViaOpenRouterAsync(userMessage, cancellationToken);

    private async Task<string> SendViaOpenRouterAsync(string userMessage, CancellationToken cancellationToken)
    {
        var client = _openRouterClient.Value
            ?? throw new InvalidOperationException(UnavailableReason);

        _openRouterHistory.Add(new AiChatMessage(AiChatRole.User, userMessage));

        var response = await client.GetResponseAsync(_openRouterHistory, cancellationToken: cancellationToken);
        var replyText = response.Text;

        _openRouterHistory.Add(new AiChatMessage(AiChatRole.Assistant, replyText));
        return replyText;
    }

    private async Task<string> SendViaOllamaServerAsync(string userMessage, CancellationToken cancellationToken)
    {
        if (!IsAvailable)
            throw new InvalidOperationException(UnavailableReason);

        _serverHistory.Add(new AiChatTurn("user", userMessage));

        var serverUrl = _appSettings.ServerUrl.TrimEnd('/');
        var httpClient = _httpClientFactory.CreateClient();

        var httpResponse = await httpClient.PostAsJsonAsync(
            $"{serverUrl}{AiChatConstants.RoutePath}",
            new AiChatRequest(_serverHistory),
            cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var payload = await httpResponse.Content.ReadFromJsonAsync<AiChatResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Empty response from the AI chat server endpoint.");

        _serverHistory.Add(new AiChatTurn("assistant", payload.Reply));
        return payload.Reply;
    }

    private const string MentionSystemPrompt =
        "You are \"AI\", a helpful participant in a group chat. You will be shown the most recent messages, " +
        "each prefixed with its sender's name. Reply naturally and concisely to the conversation, addressing " +
        "whoever mentioned you (@AI) most recently. Do not prefix your own reply with a sender name.";

    public Task<string> GetContextualReplyAsync(IReadOnlyList<ChatR.Common.ChatMessage> recentMessages, CancellationToken cancellationToken = default)
        => UsesOllamaServer
            ? GetContextualReplyViaOllamaServerAsync(recentMessages, cancellationToken)
            : GetContextualReplyViaOpenRouterAsync(recentMessages, cancellationToken);

    private async Task<string> GetContextualReplyViaOpenRouterAsync(IReadOnlyList<ChatR.Common.ChatMessage> recentMessages, CancellationToken cancellationToken)
    {
        var client = _openRouterClient.Value
            ?? throw new InvalidOperationException(UnavailableReason);

        var messages = new List<AiChatMessage> { new(AiChatRole.System, MentionSystemPrompt) };
        messages.AddRange(recentMessages.Select(m => new AiChatMessage(AiChatRole.User, $"{m.Sender}: {m.Text}")));

        var response = await client.GetResponseAsync(messages, cancellationToken: cancellationToken);
        return response.Text;
    }

    private async Task<string> GetContextualReplyViaOllamaServerAsync(IReadOnlyList<ChatR.Common.ChatMessage> recentMessages, CancellationToken cancellationToken)
    {
        if (!IsAvailable)
            throw new InvalidOperationException(UnavailableReason);

        var turns = new List<AiChatTurn> { new("system", MentionSystemPrompt) };
        turns.AddRange(recentMessages.Select(m => new AiChatTurn("user", $"{m.Sender}: {m.Text}")));

        var serverUrl = _appSettings.ServerUrl.TrimEnd('/');
        var httpClient = _httpClientFactory.CreateClient();

        var httpResponse = await httpClient.PostAsJsonAsync(
            $"{serverUrl}{AiChatConstants.RoutePath}",
            new AiChatRequest(turns),
            cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var payload = await httpResponse.Content.ReadFromJsonAsync<AiChatResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Empty response from the AI chat server endpoint.");

        return payload.Reply;
    }

    private Microsoft.Extensions.AI.IChatClient? CreateOpenRouterClient()
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return null;

        var options = new OpenAIClientOptions { Endpoint = new Uri(_settings.Endpoint) };
        var openAiClient = new OpenAIClient(new ApiKeyCredential(_settings.ApiKey), options);
        return openAiClient.GetChatClient(_settings.Model).AsIChatClient();
    }
}


