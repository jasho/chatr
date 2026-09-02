namespace ChatR.Maui.App.Services;

public interface IAppFeaturesService
{
    /// <summary>Whether the @AI mention feature is enabled on the Chat page. Can be toggled at runtime from Settings.</summary>
    bool IsAiChatEnabled { get; set; }

    /// <summary>Raised whenever <see cref="IsAiChatEnabled"/> changes, so open pages can react immediately.</summary>
    event Action<bool>? AiChatEnabledChanged;
}
