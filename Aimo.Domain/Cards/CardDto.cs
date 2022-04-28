using Aimo.Domain.Users;

namespace Aimo.Domain.Cards;
#nullable disable
#nullable enable annotations
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
    
    public ICollection<string> SubCategories { get; set; } = new List<string>();
    public ICollection<string>  Pictures { get; set; } = new List<string>();

    public DateTime DateUtc { get; set; }

    public CardType? CardType { get; set; }
}

public partial class CardDto : Dto
{
    public string Name { get; set; }
    public int CategoryId { get; set; }

    public string Address { get; set; }
    public string Zip { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public CardType? CardType { get; set; }

    public DateTime DateUtc { get; set; }
    public string PictureUrl { get; set; }
}