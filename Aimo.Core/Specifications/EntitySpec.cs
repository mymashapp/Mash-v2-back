#nullable disable
using System.Linq.Expressions;

namespace Aimo.Core.Specifications;

public sealed class ByIdsSpec<T> : Specification<T> where T : IdBase
{
    private readonly int[] _ids;

    public ByIdsSpec(params  int[] ids)
    {
        _ids = ids;
    }
    public override Expression<Func<T, bool>> ToExpression() =>  x => _ids.Contains(x.Id);
}


internal class DeletedSpecification<TEntity> : Specification<TEntity>
{
    private readonly bool _includeDeleted;

    public DeletedSpecification(bool includeDeleted = false)
    {
        _includeDeleted = includeDeleted;
    }
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return typeof(TEntity).IsAssignableTo(typeof(ISoftDeleteSupport))
            ? x => ((ISoftDeleteSupport)x).IsDeleted == _includeDeleted
            : All.ToExpression();
    }
}

internal class IsActiveSpecification<TEntity> : Specification<TEntity>
{
    private readonly bool _includeActiveOnly;

    public IsActiveSpecification(bool includeActiveOnly = true)
    {
        _includeActiveOnly = includeActiveOnly;
    }
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return typeof(TEntity).IsAssignableTo(typeof(IActiveInactiveSupport))
            ? x => ((IActiveInactiveSupport)x).IsActive == _includeActiveOnly
            : All.ToExpression();
    }
}


public static class SpecificationExtensions
{
    public static Specification<TEntity> IncludeActive<TEntity>(this Specification<TEntity> specification, bool includeActiveOnly = true)
    {
        var isActiveSpec = new IsActiveSpecification<TEntity>(includeActiveOnly);
        return specification is not null ? specification.And(isActiveSpec) : isActiveSpec;
    }

    public static Specification<TEntity> ActiveAndNotDeleted<TEntity>(this Specification<TEntity> specification)
    {
        return specification?.IncludeActive()?.IncludeDeleted() ?? Specification<TEntity>.All;
    }

    public static Specification<TEntity> IncludeDeleted<TEntity>(this Specification<TEntity> specification, bool includeDeleted = false)
    {
        var includeDeletedSpec = new DeletedSpecification<TEntity>(includeDeleted);
        return specification is not null ? specification.And(includeDeletedSpec) : includeDeletedSpec;
    }
}