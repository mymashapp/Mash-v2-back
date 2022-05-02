using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;

namespace Aimo.Data.SwipeHistories;

internal partial class ChatRepository : EfRepository<Chat>, IChatRepository
{
    public ChatRepository(IDataContext context) : base(context)
    {
    }

    public  async Task<Chat?> FirstOrDefaultAsync(Expression<Func<Chat, bool>>? predicate = null)
    {
        return await FirstOrDefaultAsync(predicate, include: x => x.Users);
    }
}

public partial interface IChatRepository : IRepository<Chat>
{
}