using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Chats;

internal partial class ChatRepository : EfRepository<Chat>, IChatRepository
{
    public ChatRepository(IDataContext context) : base(context)
    {
    }

    public async Task<Chat?> FirstOrDefaultAsync(Expression<Func<Chat, bool>>? predicate = null)
    {
        return await FirstOrDefaultAsync(predicate, include: x => x.Users);
    }

    public async Task<Chat?> GetChatWithUser(int[] userId, int cardId, GroupType groupType)
    {
        var chatUser = AsNoTracking<ChatUser>().Where(x => userId.Contains(x.UserId)).Select(x => x.ChatId);
        var chat = await Table.Where(
                x => chatUser.Contains(x.Id) && x.GroupType == groupType && x.CardId == cardId && x.Users.Count != (int)groupType
            )
            .Include(x => x.Users) 
            .FirstOrDefaultAsync();
        return chat;
    }
}

public partial interface IChatRepository : IRepository<Chat>
{
    Task<Chat?> GetChatWithUser(int[] userId, int cardId, GroupType groupType);
}