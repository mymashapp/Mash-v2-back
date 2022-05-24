using Microsoft.AspNetCore.SignalR;

namespace Aimo.Application.Chats;

public class ChatHub : Hub                                              // inherit this
{
    public Task SendMessage(string user, string message)               // Two parameters accepted
    {
        return Clients.All.SendAsync("ReceiveOne", user, message);    // Note this 'ReceiveOne' 
    }
}