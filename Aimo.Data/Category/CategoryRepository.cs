using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;

namespace Aimo.Data.Category;

internal partial class CategoryRepository : EfRepository<Domain.Category.Category>, ICategoryRepository
{
    public CategoryRepository(IDataContext context) : base(context)
    {
    }
}

public partial interface ICategoryRepository : IRepository<Domain.Category.Category>
{
}