namespace ChatR.Maui.App.Services;

/// <summary>
/// Runtime-toggleable feature flags, backed by <see cref="Preferences"/> so the choice survives
/// app restarts. <see cref="FeatureFlags"/> only supplies the initial default the first time the
/// app runs (or after clearing preferences) — after that, the Settings page switch is authoritative.
/// </summary>
public class AppFeaturesService : IAppFeaturesService
{
    private bool _isAiChatEnabled;

    public event Action<bool>? AiChatEnabledChanged;

    public AppFeaturesService()
    {
        _isAiChatEnabled = Preferences.Default.Get(PreferencesService.AiChatEnabledPreferenceKey, FeatureFlags.EnableAiInChatPage);
    }

    public bool IsAiChatEnabled
    {
        get => _isAiChatEnabled;
        set
        {
            if (_isAiChatEnabled == value)
                return;

            _isAiChatEnabled = value;
            Preferences.Default.Set(PreferencesService.AiChatEnabledPreferenceKey, value);
            AiChatEnabledChanged?.Invoke(value);
        }
    }
}
