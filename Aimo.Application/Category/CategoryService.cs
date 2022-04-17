using Aimo.Core.Specifications;
using Aimo.Data.Category;
using Aimo.Domain.Category;
using Aimo.Domain.Infrastructure;

namespace Aimo.Application.Category;

internal partial class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CategoryDtoValidator _categoryDtoValidator;


    public CategoryService(
        ICategoryRepository categoryRepository,
        CategoryDtoValidator categoryDtoValidator)
    {
        _categoryRepository = categoryRepository;
        _categoryDtoValidator = categoryDtoValidator;
    }

    #region Utilities

    #endregion

    #region Methods

    public async ResultTask GetById(int id)
    {
        var result = Result.Create(new CategoryDto());
        var entity = await _categoryRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<CategoryDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }
    public async Task<IdNameDto[]> GetAllCategory()
    {
        return (await _categoryRepository.FindAsync(x => true)).Map<IdNameDto[]>();
        
    }

    public async ResultTask Create(CategoryDto dto)
    {
        var result = await _categoryDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = dto.Map<Domain.Category.Category>();
            await _categoryRepository.AddAsync(entity);
            var affected = await _categoryRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    public async ResultTask Update(CategoryDto dto)
    {
        var result = await _categoryDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        try
        {
            var entity = await _categoryRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            dto.MapTo(entity);

            _categoryRepository.Update(entity);
            await _categoryRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto)).Success();
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
            var entity = await _categoryRepository.FindBySpecAsync(new ByIdsSpec<Domain.Category.Category>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);
            _categoryRepository.RemoveBulk(entity);
            var affected = await _categoryRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    #endregion
}

public partial interface ICategoryService
{
    Task<IdNameDto[]> GetAllCategory();
    ResultTask GetById(int id);
    ResultTask Create(CategoryDto dto);
    ResultTask Update(CategoryDto viewDto);
    Task<Result<bool>> Delete(params int[] ids);
}