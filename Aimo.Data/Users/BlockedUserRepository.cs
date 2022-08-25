using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;
using Aimo.Domain.Users;

namespace Aimo.Data.Users
{
    internal class BlockedUserRepository : EfRepository<BlockedUser>, IBlockedUserRepository
    {
        public BlockedUserRepository(IDataContext context) : base(context)
        {
        }
    }

    public partial interface IBlockedUserRepository : IRepository<BlockedUser>
    {
    }
}
