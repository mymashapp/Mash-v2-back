using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aimo.Data.Infrastructure;

/// <summary>
/// This is here as a work around to use 'dotnet ef migrations' command from migration cli
/// </summary>

#region EF_Migration_CLI

public partial class EfDataContext : IDesignTimeDbContextFactory<EfDataContext>
{
    private const string ConnectionString = "Server=mashv2.cydqrnrxyhdw.us-east-2.rds.amazonaws.com;Database=MashAppDB;User Id=admin;Password=mashv2pwd";

    public static readonly  Action<DbContextOptionsBuilder>? ContextOptionsAction = options =>
        options.UseSqlServer(ConnectionString);
    
    public static readonly DbContextOptionsBuilder<EfDataContext> ContextOptions =
        new DbContextOptionsBuilder<EfDataContext>()
        //.UseSqlServer("Server=.;Database=MashAppDB;User Id=sa;Password=admin@123").Options);
        .UseSqlServer(ConnectionString!);


    public EfDataContext()
    {
    }

    public EfDataContext CreateDbContext(string[] args) =>
        new(ContextOptions.Options);
}

#endregion