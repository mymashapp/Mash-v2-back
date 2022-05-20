#nullable disable
namespace Aimo.Application.Infrastructure;

public abstract class HttpClientBase
{
    protected static bool IsSuccessStatusCode(HttpResponseMessage httpResponse)
    {
        return (int)httpResponse.StatusCode >= 200 && (int)httpResponse.StatusCode <= 299;
    }
}