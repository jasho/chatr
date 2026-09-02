namespace ChatR.Maui.App;

/// <summary>Compile-time feature toggles, handy for demos.</summary>
public static class FeatureFlags
{
    /// <summary>
    /// Set to <see langword="false"/> to fully disable the @AI mention feature on the Chat page:
    /// hides the AI provider picker, removes the @AI hint, and stops mention detection so the
    /// chat behaves as a plain chat with no AI involvement. Flip back to <see langword="true"/>
    /// (and rebuild) to re-enable it.
    /// </summary>
    public const bool EnableAiInChatPage = true;
}
