namespace ChatR.Common;

/// <summary>One turn in an AI chat conversation, sent to/from the server's Ollama proxy endpoint.</summary>
public record AiChatTurn(string Role, string Content);

public record AiChatRequest(List<AiChatTurn> Messages);

public record AiChatResponse(string Reply);

public static class AiChatConstants
{
    public const string RoutePath = "/api/ai/chat";
}
