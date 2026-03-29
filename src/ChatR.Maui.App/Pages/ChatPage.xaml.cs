using ChatR.Maui.App.Infrastructure.Pages;
using ChatR.Maui.App.ViewModels;

namespace ChatR.Maui.App.Pages;

public partial class ChatPage : ContentPageBase<ChatPageViewModel>
{
    public ChatPage(ChatPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
