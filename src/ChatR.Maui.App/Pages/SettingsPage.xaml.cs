using ChatR.Maui.App.Infrastructure.Pages;
using ChatR.Maui.App.ViewModels;

namespace ChatR.Maui.App.Pages;

public partial class SettingsPage : ContentPageBase<SettingsPageViewModel>
{
    public SettingsPage(SettingsPageViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
