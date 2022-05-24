using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Users;

namespace Aimo.Data.Chats;

internal partial class ChatMessageRepository : EfRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(IDataContext context) : base(context)
    {
    }

}

public partial interface IChatMessageRepository : IRepository<ChatMessage>
{
}