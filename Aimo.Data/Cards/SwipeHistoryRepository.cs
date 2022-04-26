using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Cards;

internal partial class SwipeHistoryRepository : EfRepository<SwipeHistory>,ISwipeHistoryRepository
{
    public SwipeHistoryRepository(IDataContext context) : base(context)
    {
    }

   
}

public partial interface ISwipeHistoryRepository :IRepository<SwipeHistory>
{
}