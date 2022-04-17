using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Data;

namespace Aimo.Data.Card;


internal partial class CardRepository : EfRepository<Domain.Card.Card>, ICardRepository
{
    public CardRepository(IDataContext context) : base(context)
    {
    }
}

public partial interface ICardRepository : IRepository<Domain.Card.Card>
{
}

