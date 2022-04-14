using System.Data.Common;
using System.Linq.Expressions;
using Aimo.Core.Specifications;

namespace Aimo.Domain.Data;

public partial interface IRepository<TEntity> where TEntity : new()
{
    Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null);

    Task<TEntity[]> FindBySpecAsync(Specification<TEntity> spec, bool explicitControl = false);
    Task<TEntity[]> Find(Expression<Func<TEntity, bool>>? predicate = null);

    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddBulkAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    TEntity Update(TEntity entity);
    IEnumerable<TEntity> UpdateBulk(IEnumerable<TEntity> entities);
    void Remove(TEntity entity);
    void RemoveBulk(IEnumerable<TEntity> entities);
    int Count();
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
    Task<bool> NotAnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
    bool Any(Expression<Func<TEntity, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    Task<IEnumerable<TEntity>> FromSqlRaw(string sql, params object[] args);
    Task<IEnumerable<TEntity>> FromSqlInterpolated(FormattableString sql, CancellationToken ct = default);
    int ExecuteSqlInterpolated(FormattableString sql);
    Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql, CancellationToken ct = default);
    int ExecuteSqlRaw(string sql);
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default);

    #region UnitOfWork

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);

    #endregion

    Task<int> CommitAsync(DbTransaction? transaction = null, CancellationToken ct = default);


    Task<ListResult<TEntity>> ToListResultAsync<TFilter>(TFilter filter)
        where TFilter : Filter;

    Task<ListResult<TDto>> ToListResultAsync<TDto, TFilter>(TFilter filter)
        where TFilter : Filter
        where TDto : class, new();
}