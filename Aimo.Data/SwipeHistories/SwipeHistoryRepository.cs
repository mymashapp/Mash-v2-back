using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;
using Aimo.Domain.SwipeHistories;

namespace Aimo.Data.SwipeHistories;

internal partial class SwipeHistoryRepository : EfRepository<SwipeHistory>, ISwipeHistoryRepository
{
    public SwipeHistoryRepository(IDataContext context) : base(context)
    {
    }
}

public partial interface ISwipeHistoryRepository : IRepository<SwipeHistory>
{
}