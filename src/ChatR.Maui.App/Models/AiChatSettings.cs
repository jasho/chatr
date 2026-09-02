namespace ChatR.Maui.App.Models;

public class AiChatSettings
{
    public string Endpoint { get; set; } = "https://openrouter.ai/api/v1";
    public string Model { get; set; } = "openrouter/free";
    public string ApiKey { get; set; } = string.Empty;
}
