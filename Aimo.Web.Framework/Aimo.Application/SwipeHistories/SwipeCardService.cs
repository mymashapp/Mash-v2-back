using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.SwipeHistories;
using Aimo.Data.Users;
using Aimo.Domain.Chats;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.SwipeHistories;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.SwipeHistories;

internal partial class SwipeHistoryService : ISwipeHistoryService
{
    private readonly ISwipeHistoryRepository _swipeHistoryRepository;

    private readonly ISwipeGroupRepository _swipeGroupRepository;

    //private readonly IRepository<SwipeGroupInterest> _swipeGroupInterestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IChatRepository _chatRepository;
    private readonly ICardRepository _cardRepository;
    private readonly SwipeHistoryDtoValidator _swipeHistoryDtoValidator;
    private readonly SwipeGroupDtoValidator _swipeGroupDtoValidator;
    private readonly SwipeGroupInterestDtoValidator _swipeGroupInterestDtoValidator;

    public SwipeHistoryService(ISwipeHistoryRepository swipeHistoryRepository,
        ISwipeGroupRepository swipeGroupRepository,
        //  IRepository<SwipeGroupInterest> swipeGroupInterestRepository, 
        IUserRepository userRepository,
        IChatRepository chatRepository,
        ICardRepository cardRepository,
        SwipeHistoryDtoValidator swipeHistoryDtoValidator,
        SwipeGroupDtoValidator swipeGroupDtoValidator,
        SwipeGroupInterestDtoValidator swipeGroupInterestDtoValidator)
    {
        _swipeHistoryRepository = swipeHistoryRepository;
        _swipeGroupRepository = swipeGroupRepository;
        // _swipeGroupInterestRepository = swipeGroupInterestRepository;
        _userRepository = userRepository;
        _chatRepository = chatRepository;
        _cardRepository = cardRepository;
        _swipeHistoryDtoValidator = swipeHistoryDtoValidator;
        _swipeGroupDtoValidator = swipeGroupDtoValidator;
        _swipeGroupInterestDtoValidator = swipeGroupInterestDtoValidator;
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

            if (entity.SwipeType == SwipeType.Right)
                await AddSwipeGroup(entity);


            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    private async ResultTask AddSwipeGroup(SwipeHistory history)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == history.UserId).ThrowIfNull();
        var swipeGroupDto = user.MapTo(new SwipeGroupDto { CardId = history.CardId });

        var result = await _swipeGroupDtoValidator.ValidateResultAsync(swipeGroupDto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var swipeGroup = swipeGroupDto.Map<SwipeGroup>();
            await _swipeGroupRepository.AddAsync(swipeGroup);

            var affected = await _swipeGroupRepository.CommitAsync();

            if (history.SwipeType != SwipeType.Right)
                return result.SetData(swipeGroup.MapTo(swipeGroupDto), affected).Success();

            var chat = await AddChatAndChatUserAsync(history, swipeGroup, user);

            return result.SetData(chat!, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    private async Task<Chat?> AddChatAndChatUserAsync(SwipeHistory history, SwipeGroup swipeGroup, User? user)
    {
        var matchingSwipeGroup = await _swipeGroupRepository.GetMatching(swipeGroup);

        Chat? chat;
        if (matchingSwipeGroup.Any())
        {
            chat = await _chatRepository.FirstOrDefaultAsync(x =>
                x.GroupType == swipeGroup.GroupType && x.CardId == history.CardId);

            if (chat is not null)
            {
                chat.Users.Add(user!);
                _chatRepository.Update(chat);
            }

            if (chat != null && chat.Users.Count == (int)swipeGroup.GroupType)
            {
                var card = await _cardRepository.FirstOrDefaultAsync(x =>
                    x.Id == swipeGroup.CardId && x.CardType == CardType.Own);
                if (card != null) _cardRepository.Remove(card);
                await _cardRepository.CommitAsync();
            }
        }
        else
        {
            chat = new Chat
            {
                CardId = history.CardId,
                GroupType = swipeGroup.GroupType
            };
            chat.Users.Add(user!);
            await _chatRepository.AddAsync(chat);
        }

        await _chatRepository.CommitAsync();

        return chat;
    }


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


    public async ResultTask DeleteAsync(params int[] ids)
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
    }
}

public partial interface ISwipeHistoryService
{
    // Task<IdNameDto[]> GetAllSwipeHistory();
    ResultTask GetByIdAsync(int id);
    ResultTask CreateAsync(SwipeHistoryDto dto);
    ResultTask UpdateAsync(SwipeHistoryDto viewDto);
}