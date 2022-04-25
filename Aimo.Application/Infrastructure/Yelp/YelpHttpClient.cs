#nullable disable
using Aimo.Domain;
using Aimo.Domain.Cards;
using Aimo.Domain.Infrastructure;
using Marvin.StreamExtensions;
using Newtonsoft.Json;

namespace Aimo.Data.Infrastructure.Yelp;

public abstract class HttpClientBase
{
    protected static bool IsSuccessStatusCode(HttpResponseMessage httpResponse)
    {
        return (int)httpResponse.StatusCode >= 200 && (int)httpResponse.StatusCode <= 299;
    }
}

public class YelpHttpClient : HttpClientBase, IYelpHttpClient
{
    #region Utilities

    private readonly AppSetting _appSetting;
    private readonly HttpClient _httpClient;


    internal record YelpAuthToken
    {
        public string Bearer => $"Bearer {AccessToken}";
        [JsonProperty("access_token")] public string AccessToken { get; set; }
    }

    #endregion

    public YelpHttpClient(HttpClient httpClient, AppSetting appSetting)
    {
        _httpClient = httpClient;
        _appSetting = appSetting;
        _httpClient.BaseAddress = new Uri(appSetting.Yelp.ApiEndPoint);
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public async Task<ListResult<YelpCardDto>> SearchAsync(CardSearchDto dto, CancellationToken ct = default)
    {
        var result = Result.Create(new YelpCardDto[] { });
        try
        {
            var token = new YelpAuthToken { AccessToken = _appSetting.Yelp.Token };
            var searchUri =
                new Uri(
                    $"{_appSetting.Yelp.ApiEndPoint}{_appSetting.Yelp.SearchUrl.Format(dto.Category, dto.Latitude, dto.Longitude, dto.SubCategory)}");


            var httpReqMsg = new HttpRequestMessage(HttpMethod.Get, searchUri);

            httpReqMsg.Headers.Add("Authorization", token.Bearer);

            using var response = await _httpClient.SendAsync(httpReqMsg, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!IsSuccessStatusCode(response))
                return result.Failure(await response.Content.ReadAsStringAsync(ct));

            var yelpResponse = (await response.Content.ReadAsStreamAsync(ct))
                .ReadAndDeserializeFromJson<YelpRawResponse>();

            var cards = yelpResponse.Businesses.Map<YelpCardDto[]>();
            //foreach (var card in cards) card.CategoryId = dto.CategoryId;

            return result.SetData(cards).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    public async Task<ListResult<CardPictureDto>> SearchCardPictureAsync(CardPictureDto dto,
        CancellationToken ct = default)
    {
        var result = Result.Create(new CardPictureDto[] { });
        try
        {
            var token = new YelpAuthToken { AccessToken = _appSetting.Yelp.Token };
            var searchUri =
                new Uri(
                    $"{_appSetting.Yelp.ApiEndPoint}{_appSetting.Yelp.CardPictureSearchUrl.Format(dto.Alias)}");


            var httpReqMsg = new HttpRequestMessage(HttpMethod.Get, searchUri);

            httpReqMsg.Headers.Add("Authorization", token.Bearer);

            using var response = await _httpClient.SendAsync(httpReqMsg, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!IsSuccessStatusCode(response))
                return result.Failure(await response.Content.ReadAsStringAsync(ct));

            var yelpResponse = (await response.Content.ReadAsStreamAsync(ct))
                .ReadAndDeserializeFromJson<YelpRawResponsePicture>();

            var cardPicture = yelpResponse.CardPicture
                .Select(picture => new CardPictureDto { CardId = dto.CardId, PictureUrl = picture }).ToList();


            return result.SetData(cardPicture).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }
}

public interface IYelpHttpClient
{
    Task<ListResult<YelpCardDto>> SearchAsync(CardSearchDto dto, CancellationToken ct = default);
    Task<ListResult<CardPictureDto>> SearchCardPictureAsync(CardPictureDto dto, CancellationToken ct = default);
}