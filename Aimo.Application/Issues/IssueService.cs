using Aimo.Core;
using Aimo.Core.Specifications;
using Aimo.Data.Issues;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Issues;

namespace Aimo.Application.Issues;

internal partial class IssueService : IIssueService
{
    private readonly IssueDtoValidator _issueDtoValidator;
    private readonly IIssueRepository _issueRepository;

    public IssueService(IssueDtoValidator issueDtoValidator, IIssueRepository issueRepository)
    {
        _issueRepository = issueRepository;
        _issueDtoValidator = issueDtoValidator;
    }

    #region private

    #endregion

    public async Task<Result<IssueDto>> GetById(int id)
    {
        var result = Result.Create(new IssueDto());
        var entity = await _issueRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<IssueDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async Task<ListResult<IssueCountDto>> GetCountByStatus()
    {
        return Result.Create(await _issueRepository.GetIssueCountByStatusAsync()).Success();
    }

    public async Task<ListResult<IssueDto>> Find(IssueFilter filter)
    {
        return await _issueRepository.ToListResultAsync<IssueDto, IssueFilter>(filter);
    }

    public async Task<Result<IssueDto>> Create(IssueDto dto)
    {
        var result = await _issueDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = dto.Map<Issue>();

            await _issueRepository.AddAsync(entity);
            var affected = await _issueRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    public async Task<Result<IssueDto>> Update(IssueDto dto)
    {
        var result = await _issueDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = dto.Map<Issue>();

            _issueRepository.Update(entity);
            var affected = await _issueRepository.CommitAsync();

            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    public async Task<Result<bool>> Delete(params int[] ids)
    {
        var result = Result.Create(false);
        try
        {
            var entity = await _issueRepository.FindBySpecAsync(new ByIdsSpec<Issue>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);

            _issueRepository.RemoveBulk(entity);
            var affected = await _issueRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }
}

public partial interface IIssueService
{
    Task<Result<IssueDto>> GetById(int id);
    Task<ListResult<IssueCountDto>> GetCountByStatus();
    Task<ListResult<IssueDto>> Find(IssueFilter filter);
    Task<Result<IssueDto>> Create(IssueDto dto);
    Task<Result<IssueDto>> Update(IssueDto dto);
    Task<Result<bool>> Delete(params int[] ids);
}