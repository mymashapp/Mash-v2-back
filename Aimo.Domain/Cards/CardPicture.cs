namespace Aimo.Domain.Cards;

public partial class CardPicture : AuditableEntity
{
    public string PictureUrl { get; set; }
   
    public int CardId { get; set; }
    public virtual Card Card { get; set; } 

   
}
public partial class CardPictureDto 
{
   
    public string Alias { get; set; }
    public int CardId { get; set; }
    public string? PictureUrl { get; set; }
   
}