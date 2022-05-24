namespace Aimo.Domain.Chats;

public partial class ChatMessageDto : Dto
{
    public int ChatId { get; set; }
    public int CardId { get; set; }
    public string CardName { get; set; }
    public int SenderUserId { get; set; }
    public string UserName { get; set; }
    public string Text { get; set; }
    public DateTime SendOnUtc { get; set; }
}