using System.Data.Common;
using System.Reflection;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Aimo.Data.Infrastructure;

public partial class EfDataContext : DbContext, IDataContext
{
    public EfDataContext(DbContextOptions<EfDataContext> options) : base(options)
    {
    }

    #region configure

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //dynamically load all entity and query type configurations
        var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
            (type.BaseType?.IsGenericType ?? false)
            && type.BaseType.GetGenericTypeDefinition() == typeof(EntityTableMap<>));

        foreach (var typeConfiguration in typeConfigurations)
        {
            var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration)!;
            configuration?.ApplyConfiguration(modelBuilder);
        }
        base.OnModelCreating(modelBuilder);
    }

    #endregion

    #region Utilities

    public virtual int ExecuteSqlInterpolated(FormattableString sql) => Database.ExecuteSqlInterpolated(sql);

    public virtual async Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql,
        CancellationToken cancellationToken) => await Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);

    public virtual async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default) =>
        await Database.ExecuteSqlRawAsync(sql, ct);

    public virtual int ExecuteSqlRaw(string sql) => Database.ExecuteSqlRaw(sql);

    #endregion

    #region UnitOfWork

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        await UpdateAuditable();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateAuditable()
    {
        var auditables =
            ChangeTracker.Entries<AuditableEntity>().Where(e => e.State is EntityState.Modified or EntityState.Added);

        if (!auditables.Any()) return;

        var userId = (await EngineContext.Current.Resolve<IUserContext>().GetCurrentUserAsync())?.Id ?? -1;

        foreach (var entry in auditables)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                    break;
            }
        }
    }

    public async Task<int> CommitAsync(DbTransaction? transaction = null, CancellationToken ct = default)
    {
        var result = await SaveChangesAsync(ct);

        if (transaction is not null)
            await transaction.CommitAsync(ct);

        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default) => await Database.BeginTransactionAsync(ct);

    public async Task RollbackAsync(CancellationToken ct = default) => await Database.RollbackTransactionAsync(ct);

    public async Task UseTransactionAsync(DbTransaction transaction, CancellationToken ct = default) =>
        await Database.UseTransactionAsync(transaction, ct);

    #endregion
}