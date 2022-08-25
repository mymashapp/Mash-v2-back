using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.Chats;
using Aimo.Data.Users;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

namespace Aimo.Application.Chats;

internal partial class ChatMessageService : IChatMessageService
{
    private readonly IChatMessageRepository _chatMessageRepository;
    private readonly ChatMessageDtoValidator _chatMessageDtoValidator;
    private readonly IUserRepository _userRepository;
    private readonly IRepository<UserDeletedChat> _userDeletedChatRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IChatRepository _chatRepository;
    private readonly IUserContext _userContext;
    private readonly IChatUserRepository _chatUserRepository;

    public ChatMessageService(IChatMessageRepository chatMessageRepository,
        ChatMessageDtoValidator chatMessageDtoValidator,
        IUserRepository userRepository, IRepository<UserDeletedChat> userDeletedChatRepository,
        ICardRepository cardRepository, IChatRepository chatRepository, IUserContext userContext,
        IChatUserRepository chatUserRepository)
    {
        _chatMessageRepository = chatMessageRepository;
        _chatMessageDtoValidator = chatMessageDtoValidator;
        _userRepository = userRepository;
        _userDeletedChatRepository = userDeletedChatRepository;
        _cardRepository = cardRepository;
        _chatRepository = chatRepository;
        _userContext = userContext;
        _chatUserRepository = chatUserRepository;
    }


    public async Task<List<ChatMessageDto>> GetMessagesByChatId(int chatId, int userId)
    {
        var list = await _chatMessageRepository.ChatMessagesFromChatId(chatId, userId);

        return list.MapTo(new List<ChatMessageDto>());
    }

    public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new ChatMessageDto());
        var entity = await _chatMessageRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        var cardName = (await _cardRepository.FirstOrDefaultAsync(x => entity != null && x.Id == entity.Chat.CardId))
            ?.Name;

        if (entity is not null)
        {
            var chatMessageDto = entity.Map<ChatMessageDto>();
            if (cardName != null) chatMessageDto.CardName = cardName;
            return result.SetData(chatMessageDto).Success();
        }

        else
        {
            return result.Failure(ResultMessage.NotFound);
        }
    }

    public async ResultTask CreateAsync(ChatMessageDto dto)
    {
        var result = await _chatMessageDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var chat = await _chatRepository.FirstOrDefaultAsync(x => x.Id == dto.ChatId, include: y => y.Users)
                .ThrowIfNull();
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == dto.SenderUserId).ThrowIfNull();

            var entity = dto.Map<ChatMessage>();
            entity.SendOnUtc = DateTime.UtcNow;
            if (user != null) entity.User = user;
            if (chat != null) entity.Chat = chat;
            await _chatMessageRepository.AddAsync(entity);
            var affected = await _chatMessageRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }


    /*public async ResultTask UpdateAsync(ChatMessageDto dto)
    {
        var result = await _chatMessageDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        try
        {
            var entity = await _chatMessageRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            dto.MapTo(entity);

            _chatMessageRepository.Update(entity);
            await _chatMessageRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto)).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }
    */

    public async ResultTask DeleteChatAsync(int id)
    {
        var result = Result.Create(false);
        try
        {
            var currentUserId = (await _userContext.GetCurrentUserAsync()).Id;
            var chat = await _chatRepository.FirstOrDefaultAsync(x => x.Id == id);
            var chatUsers = await _chatUserRepository.GetChatUserCountForChat(id);


            var entity = new UserDeletedChat
            {
                ChatId = id,
                UserId = currentUserId
            };

            await _userDeletedChatRepository.AddAsync(entity);
            var affected = await _userDeletedChatRepository.CommitAsync();

            var deletedChat = await _userDeletedChatRepository.FindAsync(x => x.ChatId == id);

            if (chatUsers.Length == deletedChat.Length)
            {
                if (chat != null)
                    _chatUserRepository.RemoveBulk(chatUsers);
                await _chatUserRepository.CommitAsync();

                _chatRepository.Remove(chat);

                _userDeletedChatRepository.RemoveBulk(deletedChat);
                await _chatRepository.CommitAsync();
            }

            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    public async Task<List<ChatDto>> GetChatListWithUser(int userId)
    {
        return (await _chatRepository.GetChatListWithUser(userId));
    }

    public async ResultTask SendPrivateMessages(PrivateMessageDto dto)
    {
        var result = Result.Create();
        try
        {
            var chatId = 0;
            if (dto.ChatId != 0)
                chatId = dto.ChatId;

            if (dto.ChatId == 0)
                chatId = await _chatRepository.GetPrivateChatIdBetweenSenderAndReceiverAsync(dto.SenderUserId,
                    dto.ReceiverUserId);

            var chatMessageDto = dto.Map<ChatMessageDto>();

            if (chatId == 0)
            {
                var userReceiver = await _userRepository.FirstOrDefaultAsync(x => x.Id == dto.ReceiverUserId)
                    .ThrowIfNull();
                var userSender = await _userRepository.FirstOrDefaultAsync(x => x.Id == dto.SenderUserId).ThrowIfNull();
                var chat = new Chat
                {
                    CardId = AimoDefaults.PrivateMessageCardId,
                    GroupType = GroupType.None,
                    ChatType = ChatType.Private,
                };
                chat.Users.Add(userSender!);
                chat.Users.Add(userReceiver!);
                await _chatRepository.AddAsync(chat);
                await _chatRepository.CommitAsync();

                chatMessageDto.ChatId = chat.Id;
            }

            if (chatId != 0)
            {
                chatMessageDto.ChatId = chatId;
            }

            result = await CreateAsync(chatMessageDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return result;
    }

    public async Task<List<ChatMessageDto>> GetPrivateMessages(int senderId, int receiverId)
    {
        var chatId = await _chatRepository.GetPrivateChatIdBetweenSenderAndReceiverAsync(senderId, receiverId);
        var list = await _chatMessageRepository.GetPrivateMessagesByChatId(chatId);

        return list.MapTo(new List<ChatMessageDto>());
    }

    public async ResultTask GetChatByIdAsync(int chatId)
    {
        var result = Result.Create(new ChatDto());
        var entity = await _chatRepository.FirstOrDefaultAsync(x => x.Id == chatId, x => x.Users, x => x.Card);

        if (entity is not null)
        {
            var chatDto = entity.Map<ChatDto>();
            return result.SetData(chatDto).Success();
        }

        else
        {
            return result.Failure(ResultMessage.NotFound);
        }
    }

    public async Task<ChatMemberDto> GetChatMemberInfoByUserId(int userId)
    {
        return await _userRepository.GetUserWithProfilePicture(userId);
    }

    public async ResultTask RemoveChatHistoryOfBlockUser(int blockingUserId, int blockedUserId)
    {
        var result = Result.Create();
        var chatUsers =
            await _chatUserRepository.FindAsync(x => x.UserId == blockedUserId || x.UserId == blockingUserId);
        foreach (var chatUser in chatUsers)
        {
            if (chatUser.Chat.GroupType == GroupType.Two)
            {
                _chatUserRepository.Remove(chatUser);
                _chatRepository.Remove(chatUser.Chat);
            }
            else if (chatUser.Chat.GroupType == GroupType.Three && chatUser.UserId == blockingUserId)
                _chatUserRepository.Remove(chatUser);

            if (chatUser.Chat.ChatType == ChatType.Private)
            {
                _chatUserRepository.Remove(chatUser);
                _chatRepository.Remove(chatUser.Chat);
            }
        }

        await _chatRepository.CommitAsync();
        return result.Success();
    }
}

public interface IChatMessageService
{
    Task<List<ChatMessageDto>> GetMessagesByChatId(int chatId, int userId);
    ResultTask GetByIdAsync(int id);

    ResultTask CreateAsync(ChatMessageDto dto);

    // ResultTask UpdateAsync(ChatMessageDto viewDto);
    ResultTask DeleteChatAsync(int id);
    Task<List<ChatDto>> GetChatListWithUser(int userId);
    ResultTask SendPrivateMessages(PrivateMessageDto dto);
    Task<List<ChatMessageDto>> GetPrivateMessages(int senderId, int receiverId);
    ResultTask GetChatByIdAsync(int chatId);
    Task<ChatMemberDto> GetChatMemberInfoByUserId(int userId);
    ResultTask RemoveChatHistoryOfBlockUser(int blockingUserId, int blockedUserId);
}