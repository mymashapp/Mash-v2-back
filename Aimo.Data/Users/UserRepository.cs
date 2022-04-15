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

    public override async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>>? predicate = null)
    {
        var query = from user in GetQueryable<User>()
                .Include(x => x.Pictures).Include(x => x.Interests)
            /*from interestMap in Table<UserInterest>().Where(x => x.UserId == user.Id).DefaultIfEmpty()
            from interest in Table<Interest>().Where(x => x.Id == interestMap.UserId).DefaultIfEmpty()
            from picture in Table<Picture>().Where(x => x.UserId == user.Id).DefaultIfEmpty()*/
            select user;

        return predicate is not null
            ? await query.FirstOrDefaultAsync(predicate)
            : await query.FirstOrDefaultAsync();
    }
}

public partial interface IUserRepository : IRepository<User>
{
}