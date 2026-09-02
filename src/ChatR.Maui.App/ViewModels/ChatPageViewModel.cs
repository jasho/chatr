using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatR.Maui.App.ViewModels;

public partial class ChatPageViewModel : ViewModelBase
{
    private const string AiMentionToken = "@AI";
    private const string AiSender = "AI";
    private const int AiContextMessageCount = 10;

    private readonly IChatService _chatService;
    private readonly IAiChatService _aiChatService;
    private readonly IAppFeaturesService _appFeaturesService;

    [ObservableProperty]
    public partial ObservableCollection<ChatMessage> Messages { get; set; } = [];

    [ObservableProperty]
    public partial string MessageText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsConnected { get; set; }

    [ObservableProperty]
    public partial AiChatProviderOption? SelectedAiProvider { get; set; }

    [ObservableProperty]
    public partial bool IsAiEnabled { get; set; }

    public IReadOnlyList<AiChatProviderOption> AiProviders => _aiChatService.AvailableProviders;

    public string MessagePlaceholder => IsAiEnabled
        ? "Type a message... (mention @AI to ask the assistant)"
        : "Type a message...";

    public string ConnectionStatusText => IsConnected ? string.Empty : "Connecting to chat...";

    public ChatPageViewModel(IChatService chatService, IAiChatService aiChatService, IAppFeaturesService appFeaturesService)
    {
        _chatService = chatService;
        _aiChatService = aiChatService;
        _appFeaturesService = appFeaturesService;
        _chatService.MessageReceived += OnMessageReceived;
        _chatService.ConnectionStateChanged += OnConnectionStateChanged;
        _appFeaturesService.AiChatEnabledChanged += OnAiChatEnabledChanged;
        IsConnected = chatService.IsConnected;
        IsAiEnabled = _appFeaturesService.IsAiChatEnabled;
    }

    protected override async Task LoadDataAsync()
    {
        SelectedAiProvider = AiProviders.FirstOrDefault(p => p.Key == _aiChatService.Provider) ?? AiProviders.FirstOrDefault();
        await _chatService.ConnectAsync();
    }

    // Unlike the AI Chat page, switching providers here does not clear the chat history —
    // it only changes which backend answers the next @AI mention.
    partial void OnSelectedAiProviderChanged(AiChatProviderOption? value)
    {
        if (value is null)
            return;

        _aiChatService.Provider = value.Key;
    }

    partial void OnIsAiEnabledChanged(bool value)
        => OnPropertyChanged(nameof(MessagePlaceholder));

    private void OnAiChatEnabledChanged(bool isEnabled)
    {
        MainThread.BeginInvokeOnMainThread(() => IsAiEnabled = isEnabled);
    }

    public override async Task OnDisappearingAsync()
    {
        _chatService.MessageReceived -= OnMessageReceived;
        _chatService.ConnectionStateChanged -= OnConnectionStateChanged;
        _appFeaturesService.AiChatEnabledChanged -= OnAiChatEnabledChanged;
        await _chatService.DisconnectAsync();
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        var text = MessageText.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        MessageText = string.Empty;
        var savedUserName = Preferences.Default.Get(PreferencesService.UserNamePreferenceKey, string.Empty);
        var sender = string.IsNullOrWhiteSpace(savedUserName)
            ? DeviceInfo.Current.Name
            : savedUserName.Trim();

        // Snapshot the trailing context (including this message) before sending, since the
        // broadcast echo of this message may not have arrived by the time we ask the AI.
        List<ChatMessage>? aiContext = IsAiEnabled && text.Contains(AiMentionToken, StringComparison.OrdinalIgnoreCase)
            ? [.. Messages.TakeLast(AiContextMessageCount - 1), new ChatMessage(sender, text, DateTime.Now)]
            : null;

        await _chatService.SendMessageAsync(sender, text);

        if (aiContext is not null)
            await RequestAiReplyAsync(aiContext);
    }

    private async Task RequestAiReplyAsync(List<ChatMessage> recentMessages)
    {
        IsBusy = true;
        SendMessageCommand.NotifyCanExecuteChanged();
        try
        {
            var reply = await _aiChatService.GetContextualReplyAsync(recentMessages);
            await _chatService.SendMessageAsync(AiSender, reply);
        }
        catch (Exception ex)
        {
            await _chatService.SendMessageAsync(AiSender, $"Sorry, something went wrong: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
            SendMessageCommand.NotifyCanExecuteChanged();
        }
    }

    private void OnMessageReceived(ChatMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() => Messages.Add(message));
    }

    private void OnConnectionStateChanged(bool isConnected)
    {
        MainThread.BeginInvokeOnMainThread(() => IsConnected = isConnected);
    }

    private bool CanSendMessage() => IsConnected && !IsBusy;

    partial void OnIsConnectedChanged(bool value)
    {
        OnPropertyChanged(nameof(ConnectionStatusText));
        SendMessageCommand.NotifyCanExecuteChanged();
    }
}
