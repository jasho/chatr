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

    public bool HasApiKey => aiChatService.HasApiKey;

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        var text = MessageText.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        MessageText = string.Empty;
        Messages.Add(new ChatMessage(UserSender, text, DateTime.Now));

        IsBusy = true;
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
            IsBusy = false;
            SendMessageCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanSendMessage() => HasApiKey && !IsBusy;
}

