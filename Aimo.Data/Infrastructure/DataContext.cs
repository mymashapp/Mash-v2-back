using System.Data.Common;
using System.Reflection;
using Aimo.Core;
using Aimo.Domain.Data;
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
        UpdateAuditable();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditable()
    {
        var auditableEntries =
            ChangeTracker.Entries().Where(e =>
                e.Entity is AuditableEntity
                && e.State is EntityState.Modified or EntityState.Added);

        if (auditableEntries.Any())
        {
            /*var workContext = EngineContext.Current.Resolve<IWorkContext>();
            await workContext.SetCurrentUserAsync();
            var userId = (await workContext.GetCurrentUserAsync())?.Id ?? -1;*/
            var userId = -1; //TODO: fix auditable
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
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