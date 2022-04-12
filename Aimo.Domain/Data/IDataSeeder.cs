namespace Aimo.Domain.Data;

public interface IDataSeeder<TEntity> where TEntity : Entity
{
    Task Seed();
}