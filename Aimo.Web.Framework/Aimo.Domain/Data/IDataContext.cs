using System.Data.Common;

namespace Aimo.Domain.Data;

public interface IDataContext
{
    //Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
    Task<int> CommitAsync(DbTransaction? transaction = null, CancellationToken ct = default);
    Task UseTransactionAsync(DbTransaction transaction, CancellationToken ct = default);
    int ExecuteSqlInterpolated(FormattableString sql);

    Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql,
        CancellationToken cancellationToken);

    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default);
    int ExecuteSqlRaw(string sql);
}