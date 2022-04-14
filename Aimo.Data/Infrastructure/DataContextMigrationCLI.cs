using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aimo.Data.Infrastructure;

/// <summary>
/// This is here as a work around to use 'dotnet ef migrations' command from migration cli
/// </summary>

#region EF_Migration_CLI

public partial class EfDataContext : IDesignTimeDbContextFactory<EfDataContext>
{
    public EfDataContext()
    {
    }

    public EfDataContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<EfDataContext>()
            .UseSqlServer("Server=mashv2.cydqrnrxyhdw.us-east-2.rds.amazonaws.com;Database=MashAppDB;User Id=admin;Password=mashv2pwd").Options);
}

#endregion