using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Cards;

internal partial class CardRepository : EfRepository<Card>, ICardRepository
{
    public CardRepository(IDataContext context) : base(context)
    {
    }

    public async Task<CardListDto[]> SearchCards(CardSearchDto dto)
    {
        return await AsNoTracking.ProjectTo<CardListDto>().ToArrayAsync();
    }

    public async Task<Card[]> AddCardsIfNotExists(Card[] cards)
    {
        var existingCards = await AsNoTracking
            .Where(x => cards.Select(s => s.Alias).Contains(x.Alias))
            .Select(x => x.Alias).ToArrayAsync();

        var newCards = cards.Where(x => !existingCards.Contains(x.Alias)).ToArray();
        await AddBulkAsync(newCards);
        return newCards;
    }
}

public partial interface ICardRepository : IRepository<Card>
{
    Task<Card[]> AddCardsIfNotExists(Card[] cards);
    Task<CardListDto[]> SearchCards(CardSearchDto dto);
}