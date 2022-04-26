using Aimo.Domain.Categories;
using Aimo.Domain.Users;

namespace Aimo.Domain.Cards;
#nullable enable annotations

public partial class Card : AuditableEntity, ISoftDeleteSupport
{
    public string Name { get; set; }
    public string Alias { get; set; }
    public string? PictureUrl { get; set; }
    public string Url { get; set; }
    public int? ReviewCount { get; set; }
    public float? Rating { get; set; }
    public string? PhoneNo { get; set; }

    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string ZipCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public DateTime DateUtc { get; set; }
    
    public int? CategoryId { get; set; }

    public CardType? CardType { get; set; }


    public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } 
    public List<SubCategory> SubCategories { get;  init; } = new();
    public List<CardPicture> CardPictures { get;  init; } = new();

}