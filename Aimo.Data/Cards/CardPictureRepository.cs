using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Cards;

internal partial class CardPictureRepository : EfRepository<CardPicture>, ICardPictureRepository
{
    public CardPictureRepository(IDataContext context) : base(context)
    {
    }

    public async Task<CardPicture[]> AddCardsPictureIfNotExists(CardPicture[] cardPictures)
    {
        var existingCardPictures = await AsNoTracking
            .Where(x => cardPictures.Select(s => s.CardId).Contains(x.CardId))
            .Select(x => x.CardId).ToArrayAsync();

        var newCardPictures = cardPictures.Where(x => !existingCardPictures.Contains(x.CardId)).ToArray();
        await AddBulkAsync(newCardPictures);
        return newCardPictures;
    }
}

public partial interface ICardPictureRepository :IRepository<CardPicture>
{
    Task<CardPicture[]> AddCardsPictureIfNotExists(CardPicture[] cardPictures);
}