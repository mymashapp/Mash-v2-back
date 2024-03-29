#nullable disable
#nullable enable annotations
using System.Linq.Expressions;
using Aimo.Core.Specifications;
using Aimo.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Infrastructure;

internal partial class EfRepository<TEntity> : EfRepository, IRepository<TEntity> where TEntity : Entity, new()
{
    protected readonly IDataContext DataContext;

    public EfRepository(IDataContext context) : base(context) => DataContext = context;

    private DbSet<TEntity> _entity = null!;
    protected DbSet<TEntity> EntitySet => _entity ??= (DataContext as EfDataContext)!.Set<TEntity>();

    protected IQueryable<TEntity> AsNoTracking => EntitySet.AsNoTracking();
    protected IQueryable<TEntity> Table => EntitySet;

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default) =>
        (await base.GetByIdAsync<TEntity>(id, ct))!;

    public virtual async Task<TEntity[]> FindBySpecAsync(Specification<TEntity> spec,
        bool explicitControl = false) => await base.FindBySpecAsync(spec, explicitControl);

    public virtual async Task<TEntity[]> FindAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) =>
        await base.FindAsync(predicate, ct);

    public virtual async Task<ListResult<TEntity>> ToListResultAsync<TFilter>(TFilter filter)
        where TFilter : Filter =>
        await base.ToListResultAsync<TEntity, TFilter>(filter);

    public new virtual async Task<ListResult<TDto>> ToListResultAsync<TDto, TFilter>(TFilter filter)
        where TFilter : Filter
        where TDto : class, new() =>
        await base.ToListResultAsync<TEntity, TDto, TFilter>(filter);


    /*public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null) =>
        await base.FirstOrDefaultAsync(predicate);*/

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] include) =>
        await base.FirstOrDefaultAsync(predicate, include);

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
        await base.AddAsync(entity, ct);

    public virtual async Task AddBulkAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
        await base.AddBulkAsync(entities, ct);

    public virtual TEntity Update(TEntity entity) => base.Update(entity);

    public virtual IEnumerable<TEntity> UpdateBulk(IEnumerable<TEntity> entities) => base.UpdateBulk(entities);

    public virtual void RemoveBulk(IEnumerable<TEntity> entities) => base.RemoveBulk(entities);

    public virtual void Remove(TEntity entity) => base.Remove(entity);

    public virtual int Count() => base.Count<TEntity>();

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) => await base.CountAsync<TEntity>(ct: ct);


    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) => await base.AnyAsync<TEntity>(ct: ct);

    public virtual async Task<bool> NotAnyAsync(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) => await base.NotAnyAsync<TEntity>(ct: ct);


    public virtual int Count(Expression<Func<TEntity, bool>> predicate) => base.Count(predicate);

    public virtual bool Any(Expression<Func<TEntity, bool>> predicate) => base.Any(predicate);

    public virtual bool NotAny(Expression<Func<TEntity, bool>> predicate) => base.NotAny(predicate);

    public virtual async Task<IEnumerable<TEntity>> FromSqlRaw(string sql, params object[] args) =>
        await base.FromSqlRaw<TEntity>(sql, args);

    public virtual IQueryable<TEntity> FromSqlRawQueryable(string sql, params object[] args) =>
        base.FromSqlRawQueryable<TEntity>(sql, args);


    public virtual async Task<IEnumerable<TEntity>> FromSqlInterpolated(FormattableString sql,
        CancellationToken ct = default) => await base.FromSqlInterpolated<TEntity>(sql, ct);

    public virtual IQueryable<TEntity> FromSqlWithParameters(string sql, params object[] parameters) =>
        base.FromSqlWithParameters<TEntity>(sql, parameters);

    protected virtual IQueryable<TEntity> FromSqlInterpolatedQueryable(FormattableString sql) =>
        base.FromSqlInterpolatedQueryable<TEntity>(sql);

    protected static Expression<Func<TEntity, object>>[]
        IncludeAll(params Expression<Func<TEntity, object>>[] expressions) => IncludeAll<TEntity>(expressions);

    protected IQueryable<TEntity> GetQueryable(
        Expression<Func<TEntity, bool>>? predicate = null,
        int? skip = null, int? take = null,
        string? orderBy = null, SortDirection orderDirection = SortDirection.Asc,
        bool? isActive = null, bool? isDeleted = null,
        params Expression<Func<TEntity, object>>[] include)
        => GetQueryable<TEntity>(predicate, skip, take, orderBy, orderDirection, isDeleted, isActive, include);
}