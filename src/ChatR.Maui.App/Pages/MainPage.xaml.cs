using ChatR.Maui.App.Infrastructure.Pages;
using ChatR.Maui.App.ViewModels;

namespace ChatR.Maui.App.Pages;

public partial class MainPage : ContentPageBase<MainPageViewModel>
{
    public MainPage(MainPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}

