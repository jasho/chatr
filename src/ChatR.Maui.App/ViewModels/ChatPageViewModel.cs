using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatR.Maui.App.ViewModels;

public partial class ChatPageViewModel : ViewModelBase
{
    private readonly IChatService _chatService;

    [ObservableProperty]
    private ObservableCollection<ChatMessage> messages = [];

    [ObservableProperty]
    private string messageText = string.Empty;

    public ChatPageViewModel(IChatService chatService)
    {
        _chatService = chatService;
        _chatService.MessageReceived += OnMessageReceived;
    }

    protected override async Task LoadDataAsync()
    {
        await _chatService.ConnectAsync();
    }

    public override async Task OnDisappearingAsync()
    {
        _chatService.MessageReceived -= OnMessageReceived;
        await _chatService.DisconnectAsync();
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        var text = MessageText.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        MessageText = string.Empty;
        await _chatService.SendMessageAsync(DeviceInfo.Current.Name, text);
    }

    private void OnMessageReceived(ChatMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() => Messages.Add(message));
    }
}
