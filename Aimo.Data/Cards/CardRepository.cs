using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Cards;
using Aimo.Domain.Categories;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.SwipeHistories;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Cards;

internal partial class CardRepository : EfRepository<Card>, ICardRepository
{
    private readonly AppSetting _appSetting;

    public CardRepository(IDataContext context, AppSetting appSetting) : base(context)
    {
        _appSetting = appSetting;
    }

    public async Task<CardListDto[]> SearchCards(CardSearchDto dto)
    {
        Expression<Func<Card, bool>> zipEql = x => x.ZipCode == dto.ZipCode;

        var wthOutLastDigit = dto.ZipCode.Length > 1 ? $"{dto.ZipCode[..^1]}%" : dto.ZipCode;
        Expression<Func<Card, bool>> zipLike = x => EF.Functions.Like(x.ZipCode, wthOutLastDigit);


        var cardQuery = AsNoTracking;
        var categoryQuery = AsNoTracking<Category>();
        var swipeHistories = AsNoTracking<SwipeHistory>();

        var cardQueryExcludingSwipes =
            from cd in cardQuery
            join h in swipeHistories on cd.Id equals h.CardId into j
            from h in j.DefaultIfEmpty()
#pragma warning disable CS0472
            where h.CardId == null
#pragma warning restore CS0472
            select new Card { Id = cd.Id, CardType = cd.CardType, ZipCode = cd.ZipCode, CategoryId = cd.CategoryId };


        #region Yelp

        var yelpCardQuery = categoryQuery.Select(ct => ct.Id).SelectMany(categoryId =>
            cardQueryExcludingSwipes.Where(c => c.CategoryId == categoryId && c.CardType == CardType.Yelp)
                .Take(_appSetting.ItemCountPerProvider));


        //TODO: can hard code count for perf improvement
        var categoryCount = await categoryQuery.CountAsync();

        if (await yelpCardQuery.CountAsync(zipEql) > _appSetting.ItemCountPerProvider * categoryCount)
        {
            yelpCardQuery = yelpCardQuery.Where(zipEql);
        }
        else if (await yelpCardQuery.CountAsync(zipLike) > _appSetting.ItemCountPerProvider * categoryCount)
        {
            yelpCardQuery = yelpCardQuery.Where(zipLike);
        }

        #endregion

        #region GroupOn

        var groupOnCardQuery = cardQueryExcludingSwipes
            .Where(c => c.CardType == CardType.Groupon)
            .Take(_appSetting.ItemCountPerProvider);
        //.Select(cd => new Card { Id = cd.Id, ZipCode = cd.ZipCode });

        if (await groupOnCardQuery.CountAsync(zipEql) > _appSetting.ItemCountPerProvider)
        {
            groupOnCardQuery = groupOnCardQuery.Where(zipEql);
        }
        else if (await groupOnCardQuery.CountAsync(zipLike) > _appSetting.ItemCountPerProvider)
        {
            groupOnCardQuery = groupOnCardQuery.Where(zipLike);
        }

        #endregion

        #region AirBnB

        var airBnbOnCardQuery = cardQueryExcludingSwipes
            .Where(c => c.CardType == CardType.Airbnb)
            .Take(_appSetting.ItemCountPerProvider);
        //.Select(cd => new Card { Id = cd.Id, ZipCode = cd.ZipCode });

        if (await airBnbOnCardQuery.CountAsync(zipEql) > _appSetting.ItemCountPerProvider)
        {
            airBnbOnCardQuery = airBnbOnCardQuery.Where(zipEql);
        }
        else if (await airBnbOnCardQuery.CountAsync(zipLike) > _appSetting.ItemCountPerProvider)
        {
            airBnbOnCardQuery = airBnbOnCardQuery.Where(zipLike);
        }

        #endregion

        var cardIds = await yelpCardQuery.Select(x => x.Id)
            .Union(groupOnCardQuery.Select(x => x.Id))
            .Union(airBnbOnCardQuery.Select(x => x.Id)).ToArrayAsync();

        var ret = cardQuery.Where(x => cardIds.Contains(x.Id)).Include(x => x.SubCategories)
            .Include(x => x.CardPictures);


        return await ret.ProjectTo<CardListDto>()
            .ToArrayAsync();
    }

    public async Task<CardPictureDto[]> SearchCardsForPicture()
    {
        return await AsNoTracking.ProjectTo<CardPictureDto>().ToArrayAsync();
    }

    public async Task<Card[]> AddCardsIfNotExists(ICollection<Card> cards)
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
    Task<Card[]> AddCardsIfNotExists(ICollection<Card> cards);
    Task<CardListDto[]> SearchCards(CardSearchDto dto);
    Task<CardPictureDto[]> SearchCardsForPicture();
}