using Aimo.Domain.Users;

namespace Aimo.Domain.Cards;

public partial class CardListDto : Dto
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string? PictureUrl { get; set; }
    public string Url { get; set; }
    public int? ReviewCount { get; set; }
    public float? Rating { get; set; }
    public string? PhoneNo { get; set; }
    public string Address { get; set; }
    public string ZipCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Category { get; set; }
    
    public int? SwipeCount { get; set; }
    public DateTime DateUtc { get; set; }

    public CardType? CardType { get; set; }
    public ICollection<string> SubCategories { get; set; } = new List<string>();
    public ICollection<string>  Pictures { get; set; } = new List<string>();

}