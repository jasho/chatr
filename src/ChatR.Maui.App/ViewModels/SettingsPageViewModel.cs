using CommunityToolkit.Mvvm.Input;

namespace ChatR.Maui.App.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string UserName { get; set; } = string.Empty;

    public string[] ThemeOptions { get; } = [nameof(AppTheme.Light), nameof(AppTheme.Dark)];

    [ObservableProperty]
    public partial string SelectedTheme { get; set; } = nameof(AppTheme.Light);

    public SettingsPageViewModel()
    {
        var savedUserName = Preferences.Default.Get(PreferencesService.UserNamePreferenceKey, string.Empty);
        UserName = string.IsNullOrWhiteSpace(savedUserName)
            ? DeviceInfo.Current.Name
            : savedUserName;

        var savedTheme = Preferences.Default.Get(PreferencesService.ThemePreferenceKey, string.Empty);
        SelectedTheme = Enum.TryParse<AppTheme>(savedTheme, out var theme) && theme != AppTheme.Unspecified
            ? savedTheme
            : nameof(AppTheme.Light);
    }

    [RelayCommand]
    private void SaveSettings()
    {
        var trimmedValue = UserName.Trim();
        if (string.IsNullOrWhiteSpace(trimmedValue))
            trimmedValue = DeviceInfo.Current.Name;

        UserName = trimmedValue;
        Preferences.Default.Set(PreferencesService.UserNamePreferenceKey, trimmedValue);

        if (Enum.TryParse<AppTheme>(SelectedTheme, out var theme))
        {
            Preferences.Default.Set(PreferencesService.ThemePreferenceKey, SelectedTheme);
            if (Application.Current is not null)
                Application.Current.UserAppTheme = theme;
        }
    }

    protected override Task LoadDataAsync()
        => Task.CompletedTask;
}
