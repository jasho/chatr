using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatR.Maui.App.ViewModels;

public partial class ChatPageViewModel : ViewModelBase
{
    private readonly IChatService _chatService;

    [ObservableProperty]
    public partial ObservableCollection<ChatMessage> Messages { get; set; } = [];

    [ObservableProperty]
    public partial string MessageText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsConnected { get; set; }

    public string ConnectionStatusText => IsConnected ? string.Empty : "Connecting to chat...";

    public ChatPageViewModel(IChatService chatService)
    {
        _chatService = chatService;
        _chatService.MessageReceived += OnMessageReceived;
        _chatService.ConnectionStateChanged += OnConnectionStateChanged;
        IsConnected = chatService.IsConnected;
    }

    protected override async Task LoadDataAsync()
    {
        await _chatService.ConnectAsync();
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
        await _chatService.SendMessageAsync(sender, text);
    }

    private void OnMessageReceived(ChatMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() => Messages.Add(message));
    }

    private void OnConnectionStateChanged(bool isConnected)
    {
        MainThread.BeginInvokeOnMainThread(() => IsConnected = isConnected);
    }

    private bool CanSendMessage() => IsConnected;

    partial void OnIsConnectedChanged(bool value)
    {
        OnPropertyChanged(nameof(ConnectionStatusText));
        SendMessageCommand.NotifyCanExecuteChanged();
    }
}
