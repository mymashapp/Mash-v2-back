using Aimo.Data.Infrastructure;
using Aimo.Domain.Categories;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Categories;

internal partial class SubCategoryRepository : EfRepository<SubCategory>, ISubCategoryRepository
{
    public SubCategoryRepository(IDataContext context) : base(context)
    {
    }

    /*public async Task<SubCategory[]> AddCategoryIfNotExists(SubCategory[] subCategory)
    {
        var existingSubCategory = await AsNoTracking
            .Where(x => subCategory.Select(s => s.Alias).Contains(x.Alias))
            .Select(x => x.Alias).ToArrayAsync();

        var newSubCategories = subCategory.Where(x => !existingSubCategory.Contains(x.Alias)).ToArray();
        await AddBulkAsync(newSubCategories);
        await CommitAsync();
        return newSubCategories;
    }*/
    public async Task<ICollection<T>?> GetSubcategories<T>()
    {
        var query = Table.Include(x => x.Category);
        if (typeof(T) != typeof(SubCategory))
        {
            return await query.ProjectTo<T>().ToArrayAsync();
        }

        return await query.ToListAsync() as ICollection<T>;
    }
}


public partial interface ISubCategoryRepository : IRepository<SubCategory>
{
    Task<ICollection<T>?> GetSubcategories<T>();
    //Task<SubCategory[]> AddCategoryIfNotExists(SubCategory[] subCategory);
}