#nullable disable
using System.Linq.Expressions;

namespace Aimo.Core.Specifications;


public abstract class Specification<T>
{
    public static readonly Specification<T> All = new NoneSpecification<T>();


    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToFunc();
        return predicate(entity);
    }

    public abstract Expression<Func<T, bool>> ToExpression();
    
    public virtual Func<T, bool> ToFunc() => ToExpression().Compile();

    public Specification<T> And(Specification<T> specification)
    {
        if (this == All)
            return specification;
        if (specification == All)
            return this;

        return new AndSpecification<T>(this, specification);
    }

    public Specification<T> Or(Specification<T> specification)
    {
        if (this == All || specification == All)
            return All;

        return new OrSpecification<T>(this, specification);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }

    public static Specification<T> operator &(Specification<T> lhs, Specification<T> rhs) => lhs.And(rhs);
    public static Specification<T> operator |(Specification<T> lhs, Specification<T> rhs) => lhs.Or(rhs);
    public static Specification<T> operator !(Specification<T> spec) => spec.Not();

}

internal sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _right = right;
        _left = left;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var invokedExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);

        return (Expression<Func<T, bool>>)Expression.Lambda(Expression.AndAlso(leftExpression.Body, invokedExpression), leftExpression.Parameters);
    }
}

internal sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _right = right;
        _left = left;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var invokedExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);

        return (Expression<Func<T, bool>>)Expression.Lambda(Expression.OrElse(leftExpression.Body, invokedExpression), leftExpression.Parameters);
    }
}

internal sealed class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        var expression = _specification.ToExpression();
        var notExpression = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters.Single());
    }
}
internal sealed class NoneSpecification<TEntity> : Specification<TEntity>
{
    public override Expression<Func<TEntity, bool>> ToExpression() => x => true;
}
