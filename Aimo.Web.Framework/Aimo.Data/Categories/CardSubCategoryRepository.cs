using Aimo.Data.Infrastructure;
using Aimo.Domain.Categories;
using Aimo.Domain.Data;

namespace Aimo.Data.Categories;

internal partial class CardSubCategoryRepository : EfRepository<CardSubCategory>, ICardSubCategoryRepository
{
    public CardSubCategoryRepository(IDataContext context) : base(context)
    {
    }
    
    /*public async Task<CardSubCategory[]> AddCardSubCategoryIfNotExists(CardSubCategory[] subCategory)
    {
        var existingsubCategory = await AsNoTracking
            .Where(x => subCategory.Select(s => s.SubCategoryId && s.CardId)
            .Select(x => x.Alias).ToArrayAsync();

        var newSubCategories = subCategory.Where(x => !existingsubCategory.Contains(x.Alias)).ToArray();
        await AddBulkAsync(newSubCategories);
        await CommitAsync();
        return newSubCategories;
    }*/
}

public partial interface ICardSubCategoryRepository : IRepository<CardSubCategory>
{
}