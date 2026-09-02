namespace ChatR.Maui.App.Models;

public class AiChatSettings
{
    /// <summary>"OpenRouter" (direct, needs ApiKey) or "OllamaServer" (routed through ChatR.Server.App -> local Ollama).</summary>
    public string Provider { get; set; } = "OpenRouter";
    public string Endpoint { get; set; } = "https://openrouter.ai/api/v1";
    public string Model { get; set; } = "openrouter/free";
    public string ApiKey { get; set; } = string.Empty;
}
