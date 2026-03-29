using ChatR.Common;
using Microsoft.AspNetCore.SignalR;

namespace ChatR.Server.App.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string sender, string text)
    {
        var message = new ChatMessage(sender, text, DateTime.UtcNow);
        await Clients.All.SendAsync(ChatHubConstants.ReceiveMessage, message);
    }
}
