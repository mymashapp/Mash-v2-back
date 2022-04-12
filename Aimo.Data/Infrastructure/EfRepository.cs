using System.Data;
using System.Data.Common;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Aimo.Core;
using Aimo.Core.Specifications;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Aimo.Data.Infrastructure;

internal partial class EfRepository : IRepository
{
    private readonly IDataContext _dataContext;

    public EfRepository(IDataContext context) => _dataContext = context;

    private DbSet<TEntity> EntitySet<TEntity>() where TEntity : Entity =>
        (_dataContext as EfDataContext)!.Set<TEntity>(); //TODO: minor fix interface

    protected IQueryable<TEntity> AsNoTracking<TEntity>() where TEntity : Entity => EntitySet<TEntity>().AsNoTracking();
    protected IQueryable<TEntity> Table<TEntity>() where TEntity : Entity => EntitySet<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync<TEntity>(object id, CancellationToken ct = default)
        where TEntity : Entity =>
        (await EntitySet<TEntity>().FindAsync(id, ct))!;

    public virtual async Task<TEntity[]> FindBySpecAsync<TEntity>(Specification<TEntity> spec,
        bool explicitControl = false) where TEntity : Entity
    {
        var query = explicitControl
            ? EntitySet<TEntity>().Where(spec.ToExpression())
            : GetQueryable(spec.ToExpression(), isDeleted: false);

        return await query.ToArrayAsync();
    }

    public virtual async Task<TEntity[]> Find<TEntity>(Expression<Func<TEntity, bool>>? predicate = null)
        where TEntity : Entity
    {
        IQueryable<TEntity> query = EntitySet<TEntity>();
        if (predicate.IsNotNull()) query = query.Where(predicate);
        return await query.ToArrayAsync();
    }

    public virtual async Task<ListResult<TEntity>> ToListResultAsync<TEntity, TFilter>(TFilter filter)
        where TEntity : Entity, new() where TFilter : Filter
    {
        var query = GetQueryable<TEntity>();
        query = query.ApplyFilter(filter).ToPaged(filter);
        var result = await query.ToArrayAsync();
        return Result.Create(result).SetPaging(filter, query);
    }

    public virtual async Task<ListResult<TDto>> ToListResultAsync<TEntity, TDto, TFilter>(TFilter filter)
        where TEntity : Entity, new()
        where TFilter : Filter
        where TDto : class, new()
    {
        var query = GetQueryable<TEntity>();
        var result = await query.ApplyFilter(filter).ProjectTo<TDto>().ToPaged(filter).ToArrayAsync();
        return Result.Create(result).SetPaging(filter, query);
    }

    protected virtual IQueryable<TEntity> GetQueryable<TEntity>(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int? skip = null, int? take = null,
        string? orderBy = null, string orderDirection = "asc",
        bool? isDeleted = false, bool? isActive = null
    ) where TEntity : Entity
    {
        IQueryable<TEntity> query = EntitySet<TEntity>();

        if (include.IsNotNull()) query = include(query);

        if (predicate.IsNotNull()) query = query.Where(predicate);

        if (isDeleted is not null && typeof(ISoftDeleteSupport).IsAssignableFrom(typeof(TEntity)))
            query = query.Where(x => !((ISoftDeleteSupport)x).IsDeleted == isDeleted);

        if (isActive is not null && typeof(IActiveInactiveSupport).IsAssignableFrom(typeof(TEntity)))
            query = query.Where(x => ((IActiveInactiveSupport)x).IsActive == isActive);

        if (orderBy.IsNotNull()) query = query.OrderBy(orderBy, orderDirection);

        if (skip.HasValue) query = query.Skip(skip.Value);

        if (take.HasValue) query = query.Take(take.Value);

        return query;
    }

    public virtual async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null)
        where TEntity : Entity
    {
        return (await GetQueryable(predicate)!.FirstOrDefaultAsync())!;
    }

    public virtual async Task AddAsync<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : Entity
    {
        await EntitySet<TEntity>().AddAsync(entity, ct);
    }

    public virtual async Task AddBulkAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken ct = default)
        where TEntity : Entity
    {
        await EntitySet<TEntity>().AddRangeAsync(entities, ct);
    }

    public virtual TEntity Update<TEntity>(TEntity entity) where TEntity : Entity
    {
        EntitySet<TEntity>().Update(entity);
        return entity;
    }

    public virtual IEnumerable<TEntity> UpdateBulk<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
    {
        EntitySet<TEntity>().UpdateRange(entities);
        return EntitySet<TEntity>();
    }

    public virtual void RemoveBulk<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
    {
        var softdeletables = EntitySet<TEntity>().OfType<ISoftDeleteSupport>();
        if (softdeletables.IsNotEmpty())
        {
            foreach (var deletable in softdeletables)
            {
                deletable.IsDeleted = true;
                Update((deletable as TEntity)!);
            }

            return;
        }

        EntitySet<TEntity>().RemoveRange(entities);
    }

    public virtual void Remove<TEntity>(TEntity entity) where TEntity : Entity
    {
        if (entity is ISoftDeleteSupport deletable)
        {
            deletable.Delete();
            Update((deletable as TEntity)!);
            return;
        }

        EntitySet<TEntity>().Remove(entity);
    }


    public virtual int Count<TEntity>() where TEntity : Entity
    {
        return EntitySet<TEntity>().Count();
    }

    public virtual async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) where TEntity : Entity
    {
        return predicate is not null
            ? await EntitySet<TEntity>().CountAsync(predicate, ct)
            : await EntitySet<TEntity>().CountAsync(ct);
    }

    public virtual async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) where TEntity : Entity
    {
        return predicate is not null
            ? await AsNoTracking<TEntity>().AnyAsync(predicate, ct)
            : await AsNoTracking<TEntity>().AnyAsync(ct);
    }

    public virtual async Task<bool> NotAnyAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken ct = default) where TEntity : Entity
    {
        return !await AnyAsync(predicate, ct);
    }

    public virtual int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : Entity
    {
        return AsNoTracking<TEntity>().Count(predicate);
    }

    public virtual bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : Entity
    {
        return AsNoTracking<TEntity>().Any(predicate);
    }

    public virtual bool NotAny<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : Entity
    {
        return !Any(predicate);
    }

    public virtual async Task<IEnumerable<TEntity>> FromSqlRaw<TEntity>(string sql, params object[] args)
        where TEntity : Entity
    {
        return await FromSqlRawQueryable<TEntity>(sql, args).ToArrayAsync();
    }

    protected virtual IQueryable<TEntity> FromSqlRawQueryable<TEntity>(string sql, params object[] args)
        where TEntity : Entity
    {
        return EntitySet<TEntity>().FromSqlRaw(sql, args);
    }

    public virtual async Task<IEnumerable<TEntity>> FromSqlInterpolated<TEntity>(FormattableString sql,
        CancellationToken ct = default) where TEntity : Entity
    {
        return await FromSqlInterpolatedQueryable<TEntity>(sql).ToArrayAsync(ct);
    }

    public virtual int ExecuteSqlInterpolated(FormattableString sql)
    {
        return _dataContext.ExecuteSqlInterpolated(sql);
    }

    public virtual async Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql,
        CancellationToken cancellationToken)
    {
        return await _dataContext.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    public virtual async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default)
    {
        return await _dataContext.ExecuteSqlRawAsync(sql, ct);
    }

    public virtual int ExecuteSqlRaw(string sql)
    {
        return _dataContext.ExecuteSqlRaw(sql);
    }

    protected virtual IQueryable<TEntity> FromSqlWithParameters<TEntity>(string sql, params object[] parameters)
        where TEntity : Entity
    {
        return FromSqlRawQueryable<TEntity>(CreateSqlWithParameters(sql, parameters), parameters);
    }

    protected virtual IQueryable<TEntity> FromSqlInterpolatedQueryable<TEntity>(FormattableString sql)
        where TEntity : Entity
    {
        return EntitySet<TEntity>().FromSqlInterpolated(sql);
    }


    #region Utilities

    protected virtual string CreateSqlWithParameters(string sql, params object[] parameters)
    {
        //add parameters to sql
        for (var i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
        {
            if (parameters?[i] is not DbParameter parameter)
                continue;

            sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";

            //whether parameter is output
            if (parameter.Direction is ParameterDirection.InputOutput or ParameterDirection.Output)
                sql = $"{sql} output";
        }

        return sql;
    }

    #endregion

    #region UnitOfWork

    public async Task<int> CommitAsync(DbTransaction? transaction = null, CancellationToken ct = default)
    {
        return await _dataContext.CommitAsync(transaction, ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        await _dataContext.BeginTransactionAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        await _dataContext.RollbackAsync(ct);
    }

    /*public async Task UseTransactionAsync(DbTransaction transaction,CancellationToken ct = default) => await _dataContext.UseTransactionAsync(transaction,ct);*/

    #endregion
}