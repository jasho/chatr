using System.Collections.Specialized;

namespace ChatR.Maui.App.Pages;

public partial class AiChatPage : ContentPageBase<AiChatPageViewModel>
{
    public AiChatPage(AiChatPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.Messages.CollectionChanged += OnMessagesChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.Messages.CollectionChanged -= OnMessagesChanged;
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && ViewModel.Messages.Count > 0)
            Dispatcher.Dispatch(() => MessagesCollectionView.ScrollTo(ViewModel.Messages[^1], position: ScrollToPosition.End, animate: false));
    }
}
