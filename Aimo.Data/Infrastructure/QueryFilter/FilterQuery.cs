#nullable disable
#nullable enable annotations
using System.Reflection;

namespace Aimo.Data;

internal partial class DefaultFilterQuery : FilterQuery<IdBase, Filter>
{
    public DefaultFilterQuery(IQueryable<Entity> query, Filter filter) : base(query, filter)
    {
    }
}

internal abstract class FilterQuery<TEntity, TFilter> : OrderByQuery<TEntity>
    where TEntity : IdBase
    where TFilter : Filter
{
    protected readonly TFilter Filter;

    public FilterQuery(IQueryable<TEntity> query, TFilter filter) : base(query, filter, filter.SortDirection,
        filter?.SortColumn!)
    {
        Query = query;
        Filter = filter;
    }

    internal IQueryable<TEntity> ApplyQuery()
    {
        if (Filter is null) return Query;

        var filterMethodInfo = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
        var dtoProperties = typeof(TFilter).GetProperties();

        // from base class properties
        var ignoreProperties = new[]
        {
            "Ids",
            nameof(Filter.Id),
            nameof(Filter.PageIndex),
            nameof(Filter.Size),
            nameof(Filter.SortColumn),
            nameof(Filter.SortDirection)
        };

        var props = dtoProperties.Where(w => !ignoreProperties.Contains(w.Name));

        foreach (var propInfo in props)
        {
            var typePassed = false;

            if (propInfo.PropertyType == typeof(string))
                typePassed = StringMethodCall(propInfo);
            else if (propInfo.PropertyType == typeof(int?) ||
                     propInfo.PropertyType == typeof(decimal?) ||
                     propInfo.PropertyType == typeof(bool?) ||
                     propInfo.PropertyType == typeof(Guid?) ||
                     propInfo.PropertyType == typeof(DateTime?) ||
                     IsNullableEnum(propInfo.PropertyType) ||
                     propInfo.PropertyType == typeof(List<int?>) ||
                     propInfo.PropertyType == typeof(int[]) && ArrayNotEmpty<int>(propInfo) ||
                     propInfo.PropertyType == typeof(int?[]) && ArrayNotEmpty<int>(propInfo))
                typePassed = NullableMethodCall(propInfo);

            if (!typePassed) continue;

            filterMethodInfo.FirstOrDefault(w => w.Name == propInfo.Name)?.Invoke(this, null);
        }

        return Query;
    }

    private bool StringMethodCall(PropertyInfo propertyInfo)
    {
        var value = Convert.ToString(propertyInfo.GetValue(Filter, null));

        return !value.IsNullOrWhiteSpace();
    }

    private bool ArrayNotEmpty<T>(PropertyInfo propertyInfo)
    {
        var value = propertyInfo.GetValue(Filter, null);

        return value != Array.Empty<T>();
    }

    private bool NullableMethodCall(PropertyInfo propertyInfo) => propertyInfo.GetValue(Filter, null) is not null;
    private static bool IsNullableEnum(Type t) => Nullable.GetUnderlyingType(t) is { IsEnum: true };
}