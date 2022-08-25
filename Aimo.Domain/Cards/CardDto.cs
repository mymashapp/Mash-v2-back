using Aimo.Domain.Users;

namespace Aimo.Domain.Cards;
#nullable disable
#nullable enable annotations

public partial class CardDto : Dto
{
    public string Name { get; set; }
    public int CategoryId { get; set; }

    public string Address { get; set; }
    public string? Discription { get; set; }

    public string ZipCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public CardType? CardType { get; set; }
    public int CreatedBy { get; set; }
    public DateTime DateUtc { get; set; }
    public string PictureUrl { get; set; }
}