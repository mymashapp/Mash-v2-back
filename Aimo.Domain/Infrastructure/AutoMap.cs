#nullable disable
#nullable enable annotations
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Aimo.Domain.Infrastructure;

public static class AutoMap
{
    public static IMapper Mapper { get; private set; }

    public static MapperConfiguration MapperConfiguration { get; private set; }

    private static string GetNullExceptionMessage(string source) =>
        $"{nameof(AutoMap)} : {source} can not be null or empty";


    [MemberNotNull(nameof(MapperConfiguration), nameof(Mapper))]
    public static void Init(MapperConfiguration config)
    {
        MapperConfiguration = config;
        Mapper = config.CreateMapper();
    }

    #region Utilities

    [return: NotNull]
    public static TDestination Map<TDestination>(this object source)
    {
        source = source.ThrowIfNull(GetNullExceptionMessage(nameof(source)));
        return (Mapper.Map<TDestination>(source))!;
    }

    [return: NotNull]
    public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination) =>
        Map(source, destination)!;

    public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
    {
        source.ThrowIfNull(GetNullExceptionMessage(nameof(source)));
        return Mapper.Map(source, destination);
    }

    /*public static object Map(object source, Type sourceType, Type destinationType)
        {
            source.ThrowIfNullOrEmpty(GetNullExceptionMessage(nameof(source)));
            return Mapper.Map(source, sourceType, destinationType);
        }*/


    public static object Map(object source, Type sourceType, Type destinationType, object destination = default)
    {
        source.ThrowIfNull(GetNullExceptionMessage(nameof(source)));

        return destination is null
            ? Mapper.Map(source, sourceType, destinationType)
            : Mapper.Map(source, destination, sourceType, destinationType);
    }

    public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
        params Expression<Func<TDestination, object>>[] membersToExpand)
        => source.ThrowIfNull().ProjectTo(Mapper.ConfigurationProvider, membersToExpand);

    public static async Task<TDestination> MapToAsync<TSource, TDestination>(this Task<TSource> sourceAsync,
        TDestination destination)
    {
        var source = await sourceAsync;
        source.ThrowIfNull(GetNullExceptionMessage(nameof(source)));
        return Mapper.Map(source, destination);
    }

    /*public static async Task<TDestination> MapAsync<TSource,TDestination>(this Task<TSource> sourceAsync)
       {
           var source = await sourceAsync;
           source.ThrowIfNullOrEmpty(GetNullExceptionMessage(nameof(source)));
           return Mapper.Map<TDestination>(source);
       }*/

    #endregion

    #region Result Extensions

    public static object MapResultTo(this Result result, Type destinationType, object destination = default)
    {
        try
        {
            return destination is null
                ? Map(result.Data, result.Data.GetType(), destinationType)
                : Map(result.Data, result.Data.GetType(), destinationType, destination);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return default;
        }
    }

    public static T MapResultTo<T>(this Result result, T obj = default)
    {
        try
        {
            return obj is null ? (T)Map<T>(result.Data) : (T)MapTo(result.Data, obj);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return default;
        }
    }

    #endregion

    #region Result of T Extensions

    public static Result<T> CopyFrom<T, TO>(this Result<T> result, Result<TO> other) where TO : new() where T : new()
    {
        result.From(other);
        try
        {
            Map(other.Data, result.Data);
            result.AdditionalData = result.AdditionalData;
        }
        catch (AppException)
        {
        }

        return result;
    }

    #endregion
}