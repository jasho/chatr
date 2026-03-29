namespace ChatR.Maui.App.Infrastructure.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    protected bool ForceDataRefresh = true;

    [ObservableProperty]
    private bool isBusy;

    public async Task OnAppearingAsync()
    {
        if (ForceDataRefresh)
        {
            await LoadDataAsync();
            ForceDataRefresh = false;
        }
    }

    public virtual Task OnDisappearingAsync()
        => Task.CompletedTask;

    protected virtual Task LoadDataAsync()
        => Task.CompletedTask;
}
