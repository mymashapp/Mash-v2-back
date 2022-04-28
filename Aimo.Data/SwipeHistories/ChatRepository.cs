using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.SwipeHistories;

internal partial class ChatRepository : EfRepository<Chat>, IChatRepository
{
    public ChatRepository(IDataContext context) : base(context)
    {
    }

    public override async Task<Chat?> FirstOrDefaultAsync(Expression<Func<Chat, bool>>? predicate = null)
    {
        var query = from chat in GetQueryable<Chat>().Include(x => x.Users)
            select chat;

        return predicate is not null
            ? await query.FirstOrDefaultAsync(predicate)
            : await query.FirstOrDefaultAsync();
    }


}

public partial interface IChatRepository : IRepository<Chat>
{
}