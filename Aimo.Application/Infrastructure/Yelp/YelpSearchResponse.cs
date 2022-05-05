using Aimo.Domain.Users;
using Newtonsoft.Json;

namespace Aimo.Data.Infrastructure.Yelp;
#nullable disable
public class YelpCardDto : Dto
{
    private YelpSubCategory[] _subCategories = Array.Empty<YelpSubCategory>();
    
    public string Uid { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }
    public string PictureUrl { get; set; }
    public string Url { get; set; }
    public int ReviewCount { get; set; }
    public float Rating { get; set; }
    public string PhoneNo { get; set; }

    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public CardType? CardType { get; set; }
    public DateTime? DateUTC { get; set; }

    public int CategoryId { get; set; }
    public YelpSubCategory[] SubCategories
    {
        get
        {
            foreach (var subCategory in _subCategories) subCategory.CategoryId = CategoryId;
            return _subCategories;
        }
        set => _subCategories = value;
    }

}

internal record YelpRawResponse
{
    [JsonProperty("businesses")] public Business[] Businesses { get; set; } = Array.Empty<Business>();
}
internal record YelpRawResponsePicture
{
    [JsonProperty("photos")] public string[] CardPicture { get; set; } = Array.Empty<string>();
}



public record YelpSubCategory
{
    public int CategoryId { get; set; }
    [JsonProperty("alias")] public string Alias { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
}

internal record struct Coordinates
{
    [JsonProperty("latitude")] public double Latitude { get; set; }
    [JsonProperty("longitude")] public double Longitude { get; set; }
}

internal record struct Location
{
    [JsonProperty("address1")] public string Address1 { get; set; }
    [JsonProperty("address2")] public string Address2 { get; set; }
    [JsonProperty("address3")] public string Address3 { get; set; }
    [JsonProperty("city")] public string City { get; set; }
    [JsonProperty("zip_code")] public string ZipCode { get; set; }
    [JsonProperty("country")] public string Country { get; set; }
    [JsonProperty("state")] public string State { get; set; }
}

internal record Business
{
   // [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("alias")] public string Alias { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("image_url")] public string PictureUrl { get; set; }
    [JsonProperty("url")] public string Url { get; set; }
    [JsonProperty("review_count")] public int ReviewCount { get; set; }
    [JsonProperty("rating")] public double Rating { get; set; }
    [JsonProperty("phone")] public string PhoneNo { get; set; }
    [JsonProperty("categories")] public YelpSubCategory[] SubCategories { get; set; } = Array.Empty<YelpSubCategory>();

    [JsonProperty("location")] public Location Location { get; set; }
    [JsonProperty("coordinates")] public Coordinates Coordinates { get; set; }
}