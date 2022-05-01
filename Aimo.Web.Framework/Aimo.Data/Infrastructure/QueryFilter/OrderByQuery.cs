using System.Linq.Expressions;
using System.Reflection;

namespace Aimo.Data;

internal abstract class OrderByQuery<TEntity> where TEntity : IdBase
{
    protected readonly string SortColumn;
    protected readonly SortDirection SortDirection;
    protected IQueryable<TEntity> Query;

    public OrderByQuery(IQueryable<TEntity> query, Filter filter,
        SortDirection defaultSortDirection = SortDirection.Desc, string? defaultSortColumn = nameof(IdBase.Id))
    {
        Query = query;
        defaultSortColumn ??= nameof(IdBase.Id);
        var sortColumn = filter?.SortColumn ?? defaultSortColumn;

        SortColumn = FindSortingColumn($"OrderBy{sortColumn}")?.Name ?? $"OrderBy{defaultSortColumn}";
        SortDirection =
            filter?.SortDirection ??
            defaultSortDirection; //SetSortType(filter?.SortDirection ?? _defaultSortType.ToString());
    }
    
    private MethodInfo? FindSortingColumn(string sortColumn)
    {
        return GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(w => string.Equals(w.Name, sortColumn, StringComparison.InvariantCultureIgnoreCase));
    }

    /*private SortDirection SetSortType(string sortType) => sortType.ToLower() switch
    {
        "asc" => SortDirection.Asc,
        "desc" => SortDirection.Desc,
        _ => _defaultSortType
    };*/

    private IQueryable<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector) =>
        SortDirection == SortDirection.Asc ? Query.OrderBy(keySelector) : Query.OrderByDescending(keySelector);

    internal IQueryable<TEntity> ApplyOrderBy()
    {
        Query.ThrowIfNull();
            
        FindSortingColumn(SortColumn)?.Invoke(this, null);
        return Query;
    }

    protected void OrderById() => Query = OrderBy(w => w.Id);

    protected void OrderByCreatedAt()
    {
        Query = //Query is IQueryable<AuditableEntity>
            typeof(IQueryable<AuditableEntity>).IsAssignableFrom(Query.GetType())
                ? OrderBy(t => (t as AuditableEntity)!.CreatedAtUtc)
                : Query;
    }
}