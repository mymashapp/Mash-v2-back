using Aimo.Domain.Users;

namespace Aimo.Domain.Card;

public partial class CardDto : Dto
{
    public string EventName { get; set; }
    public int CategoryId { get; set; }

    public string Address { get; set; }
    public string Zip { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
    
    public CardType? CardType { get; set; }
    
    public DateTime DateUTC { get; set; }
    public string PictureUrl { get; set; }
    
    public bool IsDeleted { get; set; }

   // public virtual Category Category { get; set; }
}