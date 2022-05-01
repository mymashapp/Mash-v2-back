using Aimo.Core.Specifications;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Interests;

namespace Aimo.Application.Interests;

internal partial class InterestService:IInterestService
{
    private readonly IRepository<Interest> _interestRepository;
    private readonly InterestDtoValidator _interestDtoValidator;

    public InterestService(IRepository<Interest> interestRepository,InterestDtoValidator interestDtoValidator)
    {
        _interestRepository = interestRepository;
        _interestDtoValidator = interestDtoValidator;
    }
    
    
    public async Task<IdNameDto[]> GetAllInterests()
    {
        return (await _interestRepository.FindAsync(x => true)).Map<IdNameDto[]>();
    }
    
     public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new InterestDto());
        var entity = await _interestRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<InterestDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async ResultTask CreateAsync(InterestDto dto)
    {
        var result = await _interestDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = dto.Map<Interest>();
            await _interestRepository.AddAsync(entity);
            var affected = await _interestRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    public async ResultTask UpdateAsync(InterestDto dto)
    {
        var result = await _interestDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        try
        {
            var entity = await _interestRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            dto.MapTo(entity);

            _interestRepository.Update(entity);
            await _interestRepository.CommitAsync();
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
            var entity = await _interestRepository.FindBySpecAsync(new ByIdsSpec<Interest>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);
            _interestRepository.RemoveBulk(entity);
            var affected = await _interestRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

}

public partial interface IInterestService
{
   
    Task<IdNameDto[]> GetAllInterests();
    ResultTask GetByIdAsync(int id);
    ResultTask CreateAsync(InterestDto dto);
    ResultTask UpdateAsync(InterestDto viewDto);
}