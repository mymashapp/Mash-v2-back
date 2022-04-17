using Aimo.Domain.Infrastructure;

namespace Aimo.Application;

public static class ResultExtensions
{
    //TODO: Not working
    public static Result<T> From<T, TO>(this Result<T> result, Result<TO> other) where TO : new() where T : new()
    {
        result.From(other);
        try
        {
            AutoMap.Map(other.Data, result.Data);
            result.AdditionalData = other.AdditionalData;
        }
        catch (AppException)
        {
        }

        return result;
    }
}