using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Domain.Chats;

public partial class ChatDto : Dto
{
    public int CardId { get; set; }
    public string? CardName  { get; set; }
    public string? CardImage  { get; set; }
    public string? LastMessage  { get; set; }
    public DateTime? SendOnUtcForLastMessage { get; set; }

    public GroupType GroupType { get; set; }
    public IEnumerable<ChatMemberDto> ChatMembers { get; init; }
    //public ChatMessageDto? ChatMessages { get; set; }
}


public partial class ReMatchDto 
{
    public int CardId { get; set; }
    public int CurrentUserId { get; set; }
    public int ChatId { get; set; }
    public GroupType GroupType { get; set; }
}