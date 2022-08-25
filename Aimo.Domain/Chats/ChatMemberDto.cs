namespace Aimo.Domain.Chats;

public partial class ChatMemberDto : Dto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PictureUrl { get; set; }   
}