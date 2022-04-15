using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Users;

internal partial class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(IDataContext context) : base(context)
    {
    }

    public override Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>>? predicate = null)
    {
       return (from user in AsNoTracking
            from interestMap in AsNoTracking<UserInterest>().Where(x => x.UserId == user.Id)
            from interest in AsNoTracking<Interest>().Where(x => x.Id == interestMap.UserId)
            select user).FirstOrDefaultAsync(predicate);
    }   
    
    
}

public partial interface IUserRepository : IRepository<User>
{
}