using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.Chats;
using Aimo.Data.Notifications;
using Aimo.Data.SwipeHistories;
using Aimo.Data.Users;
using Aimo.Domain.Cards;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Notifications;
using Aimo.Domain.SwipeHistories;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.SwipeHistories;

internal partial class SwipeHistoryService : ISwipeHistoryService
{
    private readonly ISwipeHistoryRepository _swipeHistoryRepository;

    private readonly ISwipeGroupRepository _swipeGroupRepository;
    private readonly IRepository<ChatUser> _chatUserRepository;

    //private readonly IRepository<SwipeGroupInterest> _swipeGroupInterestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly ICardRepository _cardRepository;
    private readonly SwipeHistoryDtoValidator _swipeHistoryDtoValidator;
    private readonly SwipeGroupDtoValidator _swipeGroupDtoValidator;
    private readonly SwipeGroupInterestDtoValidator _swipeGroupInterestDtoValidator;
    private readonly IUserContext _userContext;
    private readonly INotificationRepository _notificationRepository;

    public SwipeHistoryService(ISwipeHistoryRepository swipeHistoryRepository,
        ISwipeGroupRepository swipeGroupRepository,
        IRepository<ChatUser> chatUserRepository,
        IUserRepository userRepository,
        IChatRepository chatRepository,
        ICardRepository cardRepository,
        SwipeHistoryDtoValidator swipeHistoryDtoValidator,
        SwipeGroupDtoValidator swipeGroupDtoValidator,
        SwipeGroupInterestDtoValidator swipeGroupInterestDtoValidator,
        IUserContext userContext, INotificationRepository notificationRepository)
    {
        _swipeHistoryRepository = swipeHistoryRepository;
        _swipeGroupRepository = swipeGroupRepository;
        _chatUserRepository = chatUserRepository;
        // _swipeGroupInterestRepository = swipeGroupInterestRepository;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _cardRepository = cardRepository;
        _swipeHistoryDtoValidator = swipeHistoryDtoValidator;
        _swipeGroupDtoValidator = swipeGroupDtoValidator;
        _swipeGroupInterestDtoValidator = swipeGroupInterestDtoValidator;
        _userContext = userContext;
        _notificationRepository = notificationRepository;
    }


    public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new SwipeHistoryDto());
        var entity = await _swipeHistoryRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<SwipeHistoryDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async ResultTask CreateAsync(SwipeHistoryDto dto)
    {
        var result = await _swipeHistoryDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        var swipeHistory = await _swipeHistoryRepository.FindAsync(x =>
            x.CardId == dto.CardId && x.UserId == dto.UserId && x.SwipeType == dto.SwipeType);
        if (swipeHistory.Any())
            return result.Failure(" you already  swipe this card ");
        try
        {
            var entity = dto.Map<SwipeHistory>();
            await _swipeHistoryRepository.AddAsync(entity);

            var affected = await _swipeHistoryRepository.CommitAsync();
            var swipGroupResult = new Result();
            if (entity.SwipeType == SwipeType.Right)
                swipGroupResult = await AddSwipeGroup(entity);


            return result.SetData(entity.MapTo(dto),
                swipGroupResult.IsSucceeded ? swipGroupResult : Array.Empty<int>()).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    private async ResultTask AddSwipeGroup(SwipeHistory history)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == history.UserId).ThrowIfNull();

        var now = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
        var dob = int.Parse(user?.DateOfBirth.ToString("yyyyMMdd") ?? string.Empty);
        var age = (now - dob) / 10000;

        var swipeGroupDto = user.MapTo(new SwipeGroupDto { CardId = history.CardId, CurrentUserAge = age });

        var result = await _swipeGroupDtoValidator.ValidateResultAsync(swipeGroupDto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var swipeGroup = swipeGroupDto.Map<SwipeGroup>();
            if (user is not null) swipeGroup.User = user;
            swipeGroup.Interests = user?.Interests!;
            await _swipeGroupRepository.AddAsync(swipeGroup);

            var affected = await _swipeGroupRepository.CommitAsync();

            if (history.SwipeType != SwipeType.Right)
                return result.SetData(swipeGroup.MapTo(swipeGroupDto), affected).Success();

            var (chat, sendNotificationToUserIds) = await AddChatAndChatUserAsync(history, swipeGroup, user);

            return Result.Create(chat!).SetData(chat!,sendNotificationToUserIds).Success();
           // return result.SetData(chat!, sendNotificationToUserIds).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    private async Task<(ChatDto, int[])> AddChatAndChatUserAsync(SwipeHistory history, SwipeGroup swipeGroup, User? user)
    {
        var matchingSwipeGroup = await _swipeGroupRepository.GetMatching(swipeGroup);
        var userIds = matchingSwipeGroup.Select(x => x.UserId).ToArray();
        var chat = await _chatRepository.GetChatWithUser(userIds, history.CardId, swipeGroup.GroupType);
        var currentUser = await _userContext.GetCurrentUserAsync();
        if (chat != null && matchingSwipeGroup.Any() && chat.Users.Count != (int)swipeGroup.GroupType)
        {
            if (user is not null)
            {
                var chatUser = new ChatUser
                {
                    ChatId = chat.Id,
                    UserId = user!.Id
                };
                await _chatUserRepository.AddAsync(chatUser);
            }


            if (chat.Users.Count == (int)swipeGroup.GroupType)
            {
                var card = await _cardRepository.FirstOrDefaultAsync(x =>
                    x.Id == swipeGroup.CardId && x.CardType == CardType.Own);
                if (card != null)
                {
                    _cardRepository.Remove(card);
                    await _cardRepository.CommitAsync();
                }
            }
        }
        else
        {
            chat = new Chat
            {
                CardId = history.CardId,
                GroupType = swipeGroup.GroupType,
                ChatType = ChatType.Card
            };
            chat.Users.Add(user!);
            await _chatRepository.AddAsync(chat);
        }

        await _chatRepository.CommitAsync();
        var sendNotificationToUserIds = new int[] { };
        if (currentUser is not null)
            sendNotificationToUserIds = await AddNotification(userIds.Where(x => x != currentUser.Id).ToArray());
        return (chat.Map<ChatDto>(), sendNotificationToUserIds);
    }

    private async Task<int[]> AddNotification(int[] userIds)
    {
        foreach (var userId in userIds)
        {
            var userNotification =await _notificationRepository.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.NotificationTypeId == (int)NotificationType.ProfileMatch);
            if (userNotification is not null)
            {
                var notification = new Notification()
                {
                    UserId = userId,
                    NotificationTypeId = (int)NotificationType.ProfileMatch,
                    Message = "your profile is match with another user"
                };
                await _notificationRepository.AddAsync(notification);
            }
        }

        await _notificationRepository.CommitAsync();
        return userIds;
    }

    /*
    public async ResultTask UpdateAsync(SwipeHistoryDto dto)
    {
        var result = await _swipeHistoryDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        try
        {
            var entity = await _swipeHistoryRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            dto.MapTo(entity);

            _swipeHistoryRepository.Update(entity);
            await _swipeHistoryRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto)).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }
    */

    public async ResultTask ReMatchUser(ReMatchDto dto)
    {
        var oldChat = await _chatRepository.FirstOrDefaultAsync(x => x.CardId == dto.CardId && x.Id == dto.ChatId);

        var result = Result.Create(oldChat);
        if (oldChat is null)
            return result.Failure("bed request");

        var alreadyMatchUser =
            await _chatUserRepository.FindAsync(x => x.ChatId == oldChat.Id && x.UserId != dto.CurrentUserId);
        var alreadyMatchUserIds = alreadyMatchUser.Select(x => x.UserId).Distinct().ToArray();

        var swipeGroup = await _swipeGroupRepository.FirstOrDefaultAsync(
            x => x.CardId == dto.CardId && x.UserId == dto.CurrentUserId);

        if (swipeGroup is null)
            return result.Failure("bed request");


        var matchingSwipeGroup = await _swipeGroupRepository.ReMatching(swipeGroup, alreadyMatchUserIds);

        if (matchingSwipeGroup.Any())
        {
            var user = matchingSwipeGroup.FirstOrDefault()?.User;

            if (user is null)
                return result.Failure("bed request");

            oldChat.Users.Add(user);
            _chatRepository.Update(oldChat);


            if (oldChat.Users.Count == (int)swipeGroup.GroupType)
            {
                var card = await _cardRepository.FirstOrDefaultAsync(x =>
                    x.Id == swipeGroup.CardId && x.CardType == CardType.Own);
                if (card != null) _cardRepository.Remove(card);
                await _cardRepository.CommitAsync();
            }

            _chatUserRepository.RemoveBulk(alreadyMatchUser);
            //  await _chatUserRepository.CommitAsync();
            await _chatRepository.CommitAsync();
        }
        else
        {
            var user = oldChat.Users.FirstOrDefault(x => x.Id == dto.CurrentUserId);
            if (user != null)
            {
                oldChat.Users.Remove(user);
                await _chatUserRepository.CommitAsync();
            }

            var chat = new Chat
            {
                CardId = dto.CardId,
                GroupType = dto.GroupType,
                ChatType = ChatType.Card
            };
            await _chatRepository.AddAsync(chat);
            await _chatRepository.CommitAsync();
            var chatUser = new ChatUser
            {
                ChatId = chat.Id,
                UserId = user.Id
            };
            await _chatUserRepository.AddAsync(chatUser);
            await _chatUserRepository.CommitAsync();

            result.SetData(chat).Success();
        }

        return result;
    }

/*public async ResultTask DeleteAsync(params int[] ids)
{
    var result = Result.Create(false);
    try
    {
        var entity = await _swipeHistoryRepository.FindBySpecAsync(new ByIdsSpec<SwipeHistory>(ids));

        if (!entity.IsNullOrEmpty())
            return result.Failure(ResultMessage.NotFound);
        _swipeHistoryRepository.RemoveBulk(entity);
        var affected = await _swipeHistoryRepository.CommitAsync();
        return result.SetData(affected > 0, affected).Success();
    }
    catch (Exception e)
    {
        return result.Exception(e);
    }
}*/
    

    public async ResultTask CardRightSwipe(CardDto dto)
    {
        var swipeHistoryDto = new SwipeHistoryDto
        {
            CardId = dto.Id,
            SeenAtUtc = DateTime.Now,
            UserId = dto.CreatedBy,
            SwipeType = SwipeType.Right
        };
        var result = await CreateAsync(swipeHistoryDto);
        return result;
    }
}

public partial interface ISwipeHistoryService
{
    // Task<IdNameDto[]> GetAllSwipeHistory();
    ResultTask GetByIdAsync(int id);

    ResultTask CreateAsync(SwipeHistoryDto dto);

    // ResultTask UpdateAsync(SwipeHistoryDto viewDto);
    ResultTask ReMatchUser(ReMatchDto dto);
    ResultTask CardRightSwipe(CardDto dto);
}