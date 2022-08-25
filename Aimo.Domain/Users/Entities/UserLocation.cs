namespace Aimo.Domain.Users.Entities;

public partial class UserLocation :Entity
{
    public int UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Distance { get; set; }
        
}