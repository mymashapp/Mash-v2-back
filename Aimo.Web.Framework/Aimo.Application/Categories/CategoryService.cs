using Aimo.Core.Specifications;
using Aimo.Data.Categories;
using Aimo.Domain.Categories;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;

namespace Aimo.Application.Categories;

internal partial class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRepository<SubCategory> _subCategoryRepository;
    private readonly CategoryDtoValidator _categoryDtoValidator;
    private readonly SubCategoryWithCategoryDtoValidator _subCategoryWithCategoryDtoValidator;


    public CategoryService(
        ICategoryRepository categoryRepository,
        IRepository<SubCategory> subCategoryRepository,
        CategoryDtoValidator categoryDtoValidator,
        SubCategoryWithCategoryDtoValidator subCategoryWithCategoryDtoValidator)
    {
        _categoryRepository = categoryRepository;
        _subCategoryRepository = subCategoryRepository;
        _categoryDtoValidator = categoryDtoValidator;
        _subCategoryWithCategoryDtoValidator = subCategoryWithCategoryDtoValidator;
    }

    #region Utilities

    #endregion

    #region Methods

    public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new CategoryDto());
        var entity = await _categoryRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<CategoryDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async Task<ListResult<CategoryDto>> GetAllCategory()
    {
        var categories = await _categoryRepository.FindAsync(x => true);
        var result = Result.Create(categories.Map<CategoryDto[]>());
        return categories.Any() ? result.Success() : result.Failure(ResultMessage.NotFound);
    }

    public async ResultTask CreateAsync(CategoryDto dto)
    {
        var result = await _categoryDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = dto.Map<Category>();
            await _categoryRepository.AddAsync(entity);
            var affected = await _categoryRepository.CommitAsync();

            //for add subcategory with category
            /*foreach (var subCategory in dto.SubCategories)
            {
                await CreateSubCategoriesAsync(subCategory, entity.Id);
            }*/

            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    private async ResultTask CreateSubCategoriesAsync(SubCategoryWithCategoryDto subCategory, int entityId)
    {
        subCategory.CategoryId = entityId;
        var result = await _subCategoryWithCategoryDtoValidator.ValidateResultAsync(subCategory);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = new SubCategory()
            {
                Alias = subCategory.Alias,
                Title = subCategory.Title,
                DisplayOrder = subCategory.DisplayOrder,
                CategoryId = entityId
            };
            //var entity = subCategory.Map<SubCategory>();
            await _subCategoryRepository.AddAsync(entity);
            var affected = await _subCategoryRepository.CommitAsync();
            return result.SetData(entity.MapTo(subCategory), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    public async ResultTask UpdateAsync(CategoryDto dto)
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
            return result.Exception(e);
        }
    }


    public async ResultTask DeleteAsync(params int[] ids)
    {
        var result = Result.Create(false);
        try
        {
            var entity = await _categoryRepository.FindBySpecAsync(new ByIdsSpec<Category>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);
            _categoryRepository.RemoveBulk(entity);
            var affected = await _categoryRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    #endregion
}

public partial interface ICategoryService
{
    Task<ListResult<CategoryDto>> GetAllCategory();
    ResultTask GetByIdAsync(int id);
    ResultTask CreateAsync(CategoryDto dto);
    ResultTask UpdateAsync(CategoryDto viewDto);
    ResultTask DeleteAsync(params int[] ids);
}