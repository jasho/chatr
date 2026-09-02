using CommunityToolkit.Mvvm.Input;

namespace ChatR.Maui.App.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string UserName { get; set; } = string.Empty;

    public SettingsPageViewModel()
    {
        var savedUserName = Preferences.Default.Get(PreferencesService.UserNamePreferenceKey, string.Empty);
        UserName = string.IsNullOrWhiteSpace(savedUserName)
            ? DeviceInfo.Current.Name
            : savedUserName;
    }

    [RelayCommand]
    private void SaveUserName()
    {
        var trimmedValue = UserName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedValue))
            trimmedValue = DeviceInfo.Current.Name;

        UserName = trimmedValue;
        Preferences.Default.Set(PreferencesService.UserNamePreferenceKey, trimmedValue);
    }

    protected override Task LoadDataAsync()
        => Task.CompletedTask;
}
