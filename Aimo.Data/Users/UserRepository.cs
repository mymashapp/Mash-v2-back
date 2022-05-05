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

    public override async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>>? predicate = null, params Expression<Func<User, object>>[] include)
    {
        return await base.FirstOrDefaultAsync(predicate, user => user.Pictures, user => user.Interests);
    }
}

public partial interface IUserRepository : IRepository<User>
{
}