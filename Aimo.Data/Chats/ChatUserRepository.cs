using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Aimo.Data.Chats
{
    internal class ChatUserRepository : EfRepository<ChatUser>, IChatUserRepository
    {
        public ChatUserRepository(IDataContext context) : base(context)
        {
        }

        public override async Task<ChatUser[]> FindAsync(Expression<Func<ChatUser, bool>>? predicate = null, CancellationToken ct = default)
        {
            return await // GetQueryable(include: IncludeAll(i => i.Comments.OrderByDescending(c => c.CreatedAtUtc)))
                AsNoTracking
                    .Include(i => i.Chat)
                    .ToArrayAsync();
        }

        public async Task<ChatUser[]> GetChatUserForDelete(int id)
        {
            return await AsNoTracking.Where(x => x.UserId == id).ToArrayAsync();;
        }
        public async Task<ChatUser[]> GetChatUserCountForChat(int chatId)
        {
            return (await Table.Where(x => x.ChatId == chatId).ToArrayAsync());
        }
    }

    public partial interface IChatUserRepository : IRepository<ChatUser>
    {
        Task<ChatUser[]> GetChatUserForDelete(int id);
        Task<ChatUser[]> GetChatUserCountForChat(int chatId);
    }
}
