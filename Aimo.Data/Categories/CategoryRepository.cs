using Aimo.Data.Infrastructure;
using Aimo.Domain.Categories;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Categories;

internal partial class CategoryRepository : EfRepository<Category>, ICategoryRepository
{
    public CategoryRepository(IDataContext context) : base(context)
    {
    }

    public async Task<SubCategoryWithCategoryDto[]> GetCategoryWithSubCategory()
    {
        var categories = await AsNoTracking.Include(x => x.SubCategories)
            .ProjectTo<SubCategoryWithCategoryDto>().ToArrayAsync();
        return categories;
    }
}

public partial interface ICategoryRepository : IRepository<Category>
{
    Task<SubCategoryWithCategoryDto[]> GetCategoryWithSubCategory();
}