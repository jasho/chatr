using Microsoft.Extensions.DependencyInjection;

namespace ChatR.Maui.App;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		ApplySavedTheme();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}

	private void ApplySavedTheme()
	{
		var savedTheme = Preferences.Default.Get(PreferencesService.ThemePreferenceKey, string.Empty);
		if (!Enum.TryParse<AppTheme>(savedTheme, out var theme) || theme == AppTheme.Unspecified)
		{
			// No preference saved yet: fall back to the system theme and persist it.
			theme = RequestedTheme == AppTheme.Dark ? AppTheme.Dark : AppTheme.Light;
			Preferences.Default.Set(PreferencesService.ThemePreferenceKey, theme.ToString());
		}

		UserAppTheme = theme;
	}
}