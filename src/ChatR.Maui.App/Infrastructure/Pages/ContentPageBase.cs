namespace ChatR.Maui.App.Infrastructure.Pages;

public abstract class ContentPageBase<TViewModel> : ContentPage
    where TViewModel : ViewModelBase
{
    protected TViewModel ViewModel { get; }

    protected ContentPageBase(TViewModel viewModel)
    {
        BindingContext = ViewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.OnAppearingAsync();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await ViewModel.OnDisappearingAsync();
    }
}
