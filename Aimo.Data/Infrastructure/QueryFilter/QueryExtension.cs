using Aimo.Core.Infrastructure;

namespace Aimo.Data;

internal static class QueryExtension
{
    #region Utilities

    private static Type GetQueryType<TEntity, TFilter>(IQueryable<TEntity> query)
        where TEntity : Entity where TFilter : Filter
    {
        var entityType = query.GetType().GenericTypeArguments[0];
        var baseQueryType = typeof(FilterQuery<,>).MakeGenericType(entityType, typeof(TFilter));

        return TypeHelper.GetAssemblyByName($"{nameof(Aimo)}.{nameof(Data)}").GetTypes()
                   .FirstOrDefault(t => baseQueryType.IsAssignableFrom(t))
               ?? typeof(DefaultFilterQuery);
    }

    #endregion

    internal static FilterQuery<TEntity, TFilter>? CreateQuery<TEntity, TFilter>(this IQueryable<TEntity> query,
        TFilter filter, Type queryType)
        where TEntity : Entity where TFilter : Filter
    {
        return Activator.CreateInstance(queryType, query, filter) as FilterQuery<TEntity, TFilter>;
    }

    internal static IQueryable<TEntity> ApplyFilter<TEntity, TFilter>(this IQueryable<TEntity> query, TFilter filter,
        Type? queryType = null)
        where TEntity : Entity where TFilter : Filter
    {
        queryType ??= GetQueryType<TEntity, TFilter>(query);
        var queryFilter = query.CreateQuery(filter, queryType);
        query = queryFilter?.ApplyQuery() ?? query;
        query = queryFilter?.ApplyOrderBy() ?? query;
        return query;
    }

    internal static async Task<IQueryable<TEntity>> ApplyFilterAsync<TEntity, TFilter>(
        this Task<IQueryable<TEntity>> queryTask, TFilter filter, Type? queryType = null)
        where TEntity : Entity where TFilter : Filter =>
        (await queryTask).ApplyFilter(filter, queryType);

    internal static IQueryable<TEntity> ToPaged<TEntity>(this IQueryable<TEntity> queryable, int pageIndex,
        int pageSize)
        where TEntity : class => queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);

    public static IQueryable<TEntity> ToPaged<TEntity>(this IQueryable<TEntity> query, Filter dto)
        where TEntity : class => ToPaged(query, dto?.PageIndex ?? 1, dto?.Size ?? AimoDefaults.DefaultPageSize);


    internal static ListResult<T> SetPaging<TEntity, T>(this ListResult<T> result, Filter dto,
        IQueryable<TEntity> query)
        where TEntity : Entity, new() where T : new()
    {
        query = query.ThrowIfNull();
        dto = dto.ThrowIfNull();

        result.SetPaging(dto?.PageIndex ?? 1, dto?.Size ?? AimoDefaults.DefaultPageSize, query.Count());
        result.SortBy = dto?.SortColumn ?? "Id";
        result.SortDirection = (dto?.SortDirection.ToString() ?? SortDirection.Desc.ToString()).ToLower();
        return result;
    }

    #region Enumerable

    public static IEnumerable<TEntity> ToPaged<TEntity>(this IEnumerable<TEntity> queryable, int pageIndex,
        int pageSize)
        where TEntity : class => queryable.Skip((pageIndex - 1) * pageSize).Take(pageSize);

    public static IEnumerable<TEntity> ToPaged<TEntity>(this IEnumerable<TEntity> query, Filter dto)
        where TEntity : class => ToPaged(query, dto?.PageIndex ?? 1, dto?.Size ?? AimoDefaults.DefaultPageSize);

    public static ListResult<T> SetPaging<TEntity, T>(this ListResult<T> result, Filter dto, IEnumerable<TEntity> query)
        where TEntity : Entity where T : new()
    {
        query = query.ThrowIfNull();
        dto = dto.ThrowIfNull();

        result.SetPaging(dto?.PageIndex ?? 1, dto?.Size ?? AimoDefaults.DefaultPageSize, query.Count());
        result.SortBy = dto?.SortColumn ?? "Id";
        result.SortDirection = (dto?.SortDirection.ToString() ?? SortDirection.Desc.ToString()).ToLower();
        return result;
    }

    #endregion
}