using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.Users;
using Aimo.Domain.Cards;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.SwipeHistorys;

internal partial class SwipeHistoryService : ISwipeHistoryService
{
    private readonly ISwipeHistoryRepository _swipeHistoryRepository;

    private readonly IRepository<SwipeGroup> _swipeGroupRepository;

    //private readonly IRepository<SwipeGroupInterest> _swipeGroupInterestRepository;
    private readonly IUserRepository _userRepository;
    private readonly SwipeHistoryDtoValidator _swipeHistoryDtoValidator;
    private readonly SwipeGroupDtoValidator _swipeGroupDtoValidator;
    private readonly SwipeGroupInterestDtoValidator _swipeGroupInterestDtoValidator;

    public SwipeHistoryService(ISwipeHistoryRepository swipeHistoryRepository,
        IRepository<SwipeGroup> swipeGroupRepository,
        //  IRepository<SwipeGroupInterest> swipeGroupInterestRepository, 
        IUserRepository userRepository,
        SwipeHistoryDtoValidator swipeHistoryDtoValidator,
        SwipeGroupDtoValidator swipeGroupDtoValidator,
        SwipeGroupInterestDtoValidator swipeGroupInterestDtoValidator)
    {
        _swipeHistoryRepository = swipeHistoryRepository;
        _swipeGroupRepository = swipeGroupRepository;
        // _swipeGroupInterestRepository = swipeGroupInterestRepository;
        _userRepository = userRepository;
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
        if(swipeHistory.Any())
            return result.Failure(" you already  swipe this card ");
        try
        {
            var entity = dto.Map<SwipeHistory>();
            await _swipeHistoryRepository.AddAsync(entity);

            var affected = await _swipeHistoryRepository.CommitAsync();
            await AddSwipeGroup(entity);
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    private async ResultTask AddSwipeGroup(SwipeHistory entity)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == entity.UserId);
        var swipeGroupDto = user.Map<SwipeGroupDto>();

        var result = await _swipeGroupDtoValidator.ValidateResultAsync(swipeGroupDto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            swipeGroupDto.Id = 0;
            var swipeGroupEntity = swipeGroupDto.Map<SwipeGroup>();
            await _swipeGroupRepository.AddAsync(swipeGroupEntity);

            var affected = await _swipeGroupRepository.CommitAsync();
            return result.SetData(entity.MapTo(swipeGroupDto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    private ResultTask AddSwipeGroupInterest()
    {
        throw new NotImplementedException();
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
            return result.Failure(e.Message);
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
            return result.Failure(e.Message);
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