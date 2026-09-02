using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace ChatR.Maui.App.ViewModels;

public partial class AiChatPageViewModel(IAiChatService aiChatService)
    : ViewModelBase
{
    private const string AssistantSender = "AI";
    private const string UserSender = "You";

    [ObservableProperty]
    public partial ObservableCollection<ChatMessage> Messages { get; set; } = [];

    [ObservableProperty]
    public partial string MessageText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial AiChatProviderOption? SelectedProvider { get; set; }

    [ObservableProperty]
    public partial bool IsAwaitingAiResponse { get; set; }

    public IReadOnlyList<AiChatProviderOption> Providers => aiChatService.AvailableProviders;

    public bool IsAvailable => aiChatService.IsAvailable;
    public string UnavailableReason => aiChatService.UnavailableReason;

    protected override Task LoadDataAsync()
    {
        SelectedProvider = Providers.FirstOrDefault(p => p.Key == aiChatService.Provider) ?? Providers.FirstOrDefault();
        return Task.CompletedTask;
    }

    partial void OnSelectedProviderChanged(AiChatProviderOption? value)
    {
        if (value is null || string.Equals(value.Key, aiChatService.Provider, StringComparison.OrdinalIgnoreCase))
            return;

        aiChatService.Provider = value.Key;
        Messages.Clear();
        IsAwaitingAiResponse = false;

        OnPropertyChanged(nameof(IsAvailable));
        OnPropertyChanged(nameof(UnavailableReason));
        SendMessageCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        var text = MessageText.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        MessageText = string.Empty;
        Messages.Add(new ChatMessage(UserSender, text, DateTime.Now));

        IsBusy = true;
        IsAwaitingAiResponse = true;
        SendMessageCommand.NotifyCanExecuteChanged();
        try
        {
            var reply = await aiChatService.SendMessageAsync(text);
            Messages.Add(new ChatMessage(AssistantSender, reply, DateTime.Now));
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage(AssistantSender, $"Sorry, something went wrong: {ex.Message}", DateTime.Now));
        }
        finally
        {
            IsAwaitingAiResponse = false;
            IsBusy = false;
            SendMessageCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanSendMessage() => IsAvailable && !IsBusy;
}
