using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;
using Aimo.Domain.Users.Entities;

namespace Aimo.Data.Users;

internal class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(IDataContext context) : base(context)
    {
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>>? predicate = null)
    {
        return await FirstOrDefaultAsync(predicate, user => user.Pictures, user => user.Interests);
    }
}

public partial interface IUserRepository : IRepository<User>
{
}