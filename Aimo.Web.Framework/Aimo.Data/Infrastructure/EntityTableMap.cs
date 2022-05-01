using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aimo.Data.Infrastructure;

public partial interface IMappingConfiguration
{
    void ApplyConfiguration(ModelBuilder modelBuilder);
}

public abstract partial class EntityTableMap<TEntity> : IEntityTypeConfiguration<TEntity>, IMappingConfiguration
    where TEntity : Entity
{
    #region Utilities

    protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder)
    {
        if (typeof(AuditableEntity).IsAssignableFrom(typeof(TEntity)))
        {
            builder
                .Property(t => (t as AuditableEntity)!.CreatedAtUtc)
                .IsRequired(false)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }

    protected virtual void PreConfigure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(typeof(TEntity).GetTypeInfo().Name);
        builder.HasKey(t => t.Id);
    }

    #endregion

    #region Methods

    public virtual void Map(EntityTypeBuilder<TEntity> builder)
    {
    }

    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        PreConfigure(builder);
        Map(builder);
        PostConfigure(builder);
    }

    public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(this);
    }

    #endregion
}