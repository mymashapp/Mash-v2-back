namespace Aimo.Domain.Users.Entities;

public partial class UserLocationDto : Dto
{
    public int UserId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string UserName { get; set; }        
    public string UserImage { get; set; }
    public double Distance { get; set; }
}