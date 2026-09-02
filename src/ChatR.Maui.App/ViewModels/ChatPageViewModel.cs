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

    [ObservableProperty]
    public partial ObservableCollection<ChatMessage> Messages { get; set; } = [];

    [ObservableProperty]
    public partial string MessageText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsConnected { get; set; }

    [ObservableProperty]
    public partial AiChatProviderOption? SelectedAiProvider { get; set; }

    public IReadOnlyList<AiChatProviderOption> AiProviders => _aiChatService.AvailableProviders;

    public string ConnectionStatusText => IsConnected ? string.Empty : "Connecting to chat...";

    public ChatPageViewModel(IChatService chatService, IAiChatService aiChatService)
    {
        _chatService = chatService;
        _aiChatService = aiChatService;
        _chatService.MessageReceived += OnMessageReceived;
        _chatService.ConnectionStateChanged += OnConnectionStateChanged;
        IsConnected = chatService.IsConnected;
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

    public override async Task OnDisappearingAsync()
    {
        _chatService.MessageReceived -= OnMessageReceived;
        _chatService.ConnectionStateChanged -= OnConnectionStateChanged;
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
        List<ChatMessage>? aiContext = text.Contains(AiMentionToken, StringComparison.OrdinalIgnoreCase)
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
