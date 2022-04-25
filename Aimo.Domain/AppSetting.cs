namespace Aimo.Domain;

public partial class AppSetting
{
    public YelpApiSetting Yelp { get; set; }
    public int ItemCountPerProvider { get; set; }
}

public class YelpApiSetting
{
    public string ApiEndPoint { get; set; }
    public string Token { get; set; }
    public string SearchUrl { get; set; }
    public string CardPictureSearchUrl { get; set; }

}