using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class ChatDto : Dto
{
    public int CardId { get; set; }
    public int UserId { get; set; }
    public int ChatId { get; set; }
    public GroupType GroupType { get; set; }
    public List<User> Users { get; init; } 
    public List<ChatUser> ChatUser { get; init; }
}