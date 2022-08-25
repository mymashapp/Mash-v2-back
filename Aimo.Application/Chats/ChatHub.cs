using System.Text.RegularExpressions;
using Aimo.Data.Chats;
using Aimo.Data.Notifications;
using Aimo.Data.Users;
using Aimo.Domain.Chats;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.AspNetCore.SignalR;

namespace Aimo.Application.Chats;

public class ChatHub : Hub // inherit this
{
    private static readonly List<UserDto> Connections = new List<UserDto>();
    private static readonly Dictionary<string, string> ConnectionsMap = new Dictionary<string, string>();

    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly INotificationRepository _notificationRepository;
    public ChatHub(IUserContext userContext, IUserRepository userRepository, IChatRepository chatRepository,
        INotificationRepository notificationRepository)
    {
        _userContext = userContext;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task SendPrivate(string receiverName, string message)
    {
        if (ConnectionsMap.TryGetValue(receiverName, out string? userId))
        {
            // Who is the sender;
            var currentUser = await GetCurrentUser();
            var sender = Connections.First(u => u.Email == currentUser);

            if (!string.IsNullOrEmpty(message.Trim()))
            {
                // Build the message
                var messageDto = new ChatMessageDto()
                {
                    Text = Regex.Replace(message, @"<.*?>", string.Empty),
                    UserName = sender.Name,
                    //  Avatar = sender.Avatar,
                    CardName = "",
                    SendOnUtc = DateTime.UtcNow
                };

                // Send the message
                await Clients.Client(userId).SendAsync("newMessage", messageDto);
                await Clients.Caller.SendAsync("newMessage", messageDto);
            }
        }
    }

    public async Task Join(int roomName)
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var user = Connections.FirstOrDefault(u => u.Email == currentUser);
            if (user != null && user.CurrentChat != roomName.ToString())
            {
                // Remove user from others list
                if (!string.IsNullOrEmpty(user.CurrentChat))
                    await Clients.OthersInGroup(user.CurrentChat).SendAsync("removeUser", user);

                if (!string.IsNullOrEmpty(user.CurrentChat))
                    await Leave(user.CurrentChat);

                // Join to new chat room
                //await Leave(user.CurrentChat);
                if (user.Chats.Any(x => x.Id == roomName))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName.ToString());
                    user.CurrentChat = roomName.ToString();

                // Tell others to update their list of users
                await Clients.OthersInGroup(roomName.ToString()).SendAsync("addUser", user);
                }
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
        }
    }

    private async Task Leave(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public IEnumerable<UserDto> GetUsers(string roomName)
    {
        return Connections.Where(u => u.CurrentChat == roomName).ToList();
    }

    public override async Task<Task> OnConnectedAsync()
    {
        try
        {
            var currentUser = await GetCurrentUser();
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == currentUser).ThrowIfNull();
            var userDto = user?.Map<UserDto>();
            if (userDto is not null)
            {
                userDto.Device = GetDevice();
                if (user is not null) userDto.Chats = await GetChatList(user.Id);

                if (Connections.All(u => u.Email != currentUser))
                {
                    Connections.Add(userDto);
                    ConnectionsMap[currentUser] = Context.ConnectionId;
                }

                var notification = await _notificationRepository.FirstOrDefaultAsync(x => x.UserId == user.Id && x.NotificationTypeId == (int)NotificationType.ProfileMatch);
                if(notification is not null)
                {
                    await Clients.Group(user.Id.ToString()).SendAsync("ReceiveNotification", notification.Message);
                }
                await Clients.Caller.SendAsync("onConnect", userDto.Name, userDto.Id);
            }
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
        }

        return base.OnConnectedAsync();
    }

    private async Task<string> GetCurrentUser()
    {
        return (await _userContext.GetCurrentUserAsync())?.Email ?? string.Empty;
    }

    private async Task<List<ChatDto>> GetChatList(int userId)
    {
        return await _chatRepository.GetChatListWithUser(userId);
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var currentUse = await GetCurrentUser();
            var user = Connections.First(u => u.Email == currentUse);
            Connections.Remove(user);

            // Tell other users to remove you from their list
            await Clients.OthersInGroup(user.CurrentChat).SendAsync("removeUser", user);

            // Remove mapping
            ConnectionsMap.Remove(user.Name);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
        }

        await base.OnDisconnectedAsync(exception);
    }

    //  private string IdentityName => "Robert lanigan"; //Context.User?.Identity?.Name;

    private string GetDevice()
    {
        var device = Context.GetHttpContext()?.Request.Headers["Device"].ToString();
        if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile")))
            return device;

        return "Web";
    }
}