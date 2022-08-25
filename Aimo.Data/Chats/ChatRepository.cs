using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Chats;

internal partial class ChatRepository : EfRepository<Chat>, IChatRepository
{
    private readonly IDbContextFactory<EfDataContext> _parallelContextFactory;

    public ChatRepository(IDataContext context, IDbContextFactory<EfDataContext> parallelContextFactory) : base(context)
    {
        _parallelContextFactory = parallelContextFactory;
    }

    public async Task<Chat?> FirstOrDefaultAsync(Expression<Func<Chat, bool>>? predicate = null)
    {
        return await FirstOrDefaultAsync(predicate, x => x.Users, x => x.Card);
    }

    public async Task<Chat?> GetChatWithUser(int[] userId, int cardId, GroupType groupType)
    {
        var chatUser = AsNoTracking<ChatUser>().Where(x => userId.Contains(x.UserId)).Select(x => x.ChatId);
        var chat = await Table.Where(
                x => chatUser.Contains(x.Id) && x.GroupType == groupType && x.CardId == cardId &&
                     x.Users.Count != (int)groupType
            )
            .Include(x => x.Users)
            .FirstOrDefaultAsync();
        return chat;
    }

    private static Expression<Func<Chat, bool>> ChatForUsers(int[] chatUser) => x => chatUser.Contains(x.Id);

    private static IQueryable<LastMessage> GetLastChatMessageQuery(IQueryable<Chat> chatQuery,
        IQueryable<ChatMessage> chatMessages)
    {
        return from c in chatQuery
            from cm in chatMessages
                .Where(m => c.Id == m.ChatId && m.Text != null)
                .OrderByDescending(v => v.SendOnUtc).Take(1).DefaultIfEmpty()
            select new LastMessage
            {
                Text = cm != null ? cm.Text : string.Empty, SendOnUtc = cm != null ? cm.SendOnUtc : default,
                ChatId = cm != null ? cm.ChatId : 0
            };
    }


    private async Task<IQueryable<LastMessage>> GetLastMessagesQueryParallel(int[] chatUser)
    {
        var parallelContext = await _parallelContextFactory.CreateDbContextAsync();
        var chatQueryP = parallelContext.Set<Chat>().AsNoTracking().Where(ChatForUsers(chatUser));
        var chatMessagesP = parallelContext.Set<ChatMessage>().AsNoTracking();
        var lastMessagesQuery = from c in chatQueryP
            from cm in chatMessagesP
                .Where(m => c.Id == m.ChatId && m.Text != null)
                .OrderByDescending(v => v.SendOnUtc).Take(1).DefaultIfEmpty()
            select new LastMessage
            {
                Text = cm != null ? cm.Text : string.Empty, SendOnUtc = cm != null ? cm.SendOnUtc : default,
                ChatId = cm != null ? cm.ChatId : 0
            };
        return lastMessagesQuery;
    }

    public async Task<List<ChatDto>> GetChatListWithUser(int userId)
    {
        var chatUser = AsNoTracking<ChatUser>().Where(x => x.UserId == userId).Select(x => x.ChatId).ToArray();

        var chatQuery = AsNoTracking.Where(ChatForUsers(chatUser));

        var query = chatQuery.Include(x => x.Card)
            .Include(x => x.Users)
            .ThenInclude(x => x.Pictures.Where(y => y.PictureType == PictureType.ProfilePicture))
            .Select(x =>
                new ChatDto
                {
                    Id = x.Id,
                    CardId = x.Card.Id,
                    CardName = x.Card.Name,
                    CardImage = x.Card.PictureUrl,
                    GroupType = x.GroupType,
                    ChatMembers = x.Users.Select(u => new ChatMemberDto()
                    {
                        PictureUrl = u.Pictures.First(z => z.PictureType == PictureType.ProfilePicture).Url,
                        Name = u.Name,
                        Email = u.Email,
                        Id = u.Id
                    })
                });
        var resultTask = query.AsSplitQuery().ToListAsync();

        var lastMessagesQueryP = await GetLastMessagesQueryParallel(chatUser);
        var lastMessagesTask = lastMessagesQueryP.ToArrayAsync();

        await Task.WhenAll(resultTask, lastMessagesTask);

        var result = await resultTask;
        var lastMessages = await lastMessagesTask;

        /*var chatMessages = AsNoTracking<ChatMessage>();// non-parallel
        var lastMessages = GetLastChatMessageQuery(chatQuery, chatMessages);*/

        Parallel.ForEach(result, x =>
        {
            x.LastMessage = lastMessages.FirstOrDefault(y => y.ChatId == x.Id)?.Text;
            x.SendOnUtcForLastMessage = lastMessages.FirstOrDefault(y => y.ChatId == x.Id)?.SendOnUtc;
        });
        /*result.ForEach(x => { x.LastMessage = lastMessages.FirstOrDefault(y => y.ChatId == x.Id)?.Text; });*/
        return result;
    }


    public async Task<int> GetPrivateChatIdBetweenSenderAndReceiverAsync(int senderId, int receiverId)
    {
        var privateChatUserQuery =
            from c in AsNoTracking.Where(x => x.ChatType == ChatType.Private)
            from cu in AsNoTracking<ChatUser>().Where(sender => sender.ChatId == c.Id)
            select cu;

        var query =
            from sender in privateChatUserQuery.Where(x => x.UserId == senderId)
            from receiver in privateChatUserQuery.Where(x => x.UserId == receiverId && sender.ChatId == x.ChatId)
            select sender.ChatId;

        return (int?)await query.FirstOrDefaultAsync() ?? 0;
    }
}

public class LastMessage
{
    public string Text { get; set; }
    public DateTime SendOnUtc { get; set; }
    public int ChatId { get; set; }
}

public partial interface IChatRepository : IRepository<Chat>
{
    Task<Chat?> GetChatWithUser(int[] userId, int cardId, GroupType groupType);
    Task<List<ChatDto>> GetChatListWithUser(int userId);
    Task<int> GetPrivateChatIdBetweenSenderAndReceiverAsync(int senderId, int receiverId);
    Task<Chat?> FirstOrDefaultAsync(Expression<Func<Chat, bool>>? predicate = null);
}