namespace Aimo.Domain.Cards;
#nullable disable
public class CardSearchDto
{
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ZipCode { get; set; }

}