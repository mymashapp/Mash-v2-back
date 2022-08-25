using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Chats;

internal partial class ChatMessageRepository : EfRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(IDataContext context) : base(context)
    {
    }

    public override async Task<ChatMessage?> FirstOrDefaultAsync(Expression<Func<ChatMessage, bool>>? predicate = null,
        params Expression<Func<ChatMessage, object>>[] include)
    {
        return await base.FirstOrDefaultAsync(predicate, x => x.User, x => x.Chat);
    }

    public async Task<List<ChatMessage>> ChatMessagesFromChatId(int chatId,int userId)
    {
       // var deletedChat = AsNoTracking<UserDeletedChat>().Where(x => x.UserId == userId).Select(x=>x.ChatId).ToArray();
        var userJoinDate = AsNoTracking<ChatUser>().FirstOrDefault(x => x.ChatId == chatId && x.UserId==userId)?.JoinOnUtc;
        return await AsNoTracking.Where(x => x.ChatId == chatId /*&& !deletedChat.Contains(x.ChatId) */ && x.SendOnUtc >= userJoinDate && !x.IsDeleted)
            .Include(x => x.User)
            .OrderBy(m => m.SendOnUtc)
            .ToListAsync();
    }

    public async Task<List<ChatMessage>> GetPrivateMessagesByChatId(int chatId)
    {
        return await AsNoTracking.Where(x => x.ChatId == chatId &&!x.IsDeleted)
            .Include(x => x.User)
            .Include(x => x.Chat).OrderBy(m => m.SendOnUtc)
            .ToListAsync();
    }
}

public partial interface IChatMessageRepository : IRepository<ChatMessage>
{
    Task<List<ChatMessage>> ChatMessagesFromChatId(int chatId,int userId);
    Task<List<ChatMessage>> GetPrivateMessagesByChatId(int chatId);
}