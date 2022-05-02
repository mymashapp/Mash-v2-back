using System.Data.Common;
using System.Linq.Expressions;

namespace Aimo.Domain.Data;

public partial interface IRepository
{
    Task<TEntity?> GetByIdAsync<TEntity>(int id, CancellationToken ct = default) where TEntity : Entity;

    Task<TEntity?> FirstOrDefaultAsync<TEntity>(
        Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] include)
        where TEntity : Entity;

    /*Task<TEntity[]> FindBySpecAsync<TEntity>(Specification<TEntity> spec, bool explicitControl = false)
        where TEntity : Entity;*/

    Task<TEntity[]> FindAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) where TEntity : Entity;

    Task AddAsync<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : Entity;
    Task AddBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken ct = default) where TEntity : Entity;
    TEntity Update<TEntity>(TEntity entity) where TEntity : Entity;
    IEnumerable<TEntity> UpdateBulk<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity;
    void Remove<TEntity>(TEntity entity) where TEntity : Entity;
    void RemoveBulk<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity;
    int Count<TEntity>() where TEntity : Entity;

    Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        where TEntity : Entity;

    Task<bool> NotAnyAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        where TEntity : Entity;

    bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : Entity;

    Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default)
        where TEntity : Entity;

    Task<IEnumerable<TEntity>> FromSqlRaw<TEntity>(string sql, params object[] args) where TEntity : Entity;

    Task<IEnumerable<TEntity>> FromSqlInterpolated<TEntity>(FormattableString sql, CancellationToken ct = default)
        where TEntity : Entity;

    int ExecuteSqlInterpolated(FormattableString sql);
    Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql, CancellationToken ct = default);
    int ExecuteSqlRaw(string sql);
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default);

    #region UnitOfWork

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    Task<int> CommitAsync(DbTransaction? transaction = null, CancellationToken ct = default);

    #endregion


    Task<ListResult<TEntity>> ToListResultAsync<TEntity, TFilter>(TFilter filter)
        where TEntity : Entity, new() where TFilter : Filter;

    Task<ListResult<TDto>> ToListResultAsync<TEntity, TDto, TFilter>(TFilter filter)
        where TEntity : Entity, new()
        where TFilter : Filter
        where TDto : class, new();
}