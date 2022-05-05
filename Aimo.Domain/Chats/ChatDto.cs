using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class ChatDto : Dto
{
    public int CardId { get; set; }
    public int UserId { get; set; }
    public int ChatId { get; set; }
    public GroupType GroupType { get; set; }
    public List<User> Users { get; init; } = new List<User>();
    public List<ChatUser> ChatUser { get; init; }
}

public partial class ReMatchDto 
{
    public int CardId { get; set; }
    public int CurrentUserId { get; set; }
    public int ChatId { get; set; }
    public GroupType GroupType { get; set; }
}