using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Data.Users;
using Aimo.Domain.Cards;
using Aimo.Domain.Categories;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.SwipeHistories;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Cards;

internal partial class CardRepository : EfRepository<Card>, ICardRepository
{
    private readonly AppSetting _appSetting;
    private readonly IUserContext _userContext;

    public CardRepository(IDataContext context, AppSetting appSetting,
        IUserContext userContext) : base(context)
    {
        _appSetting = appSetting;
        _userContext = userContext;
    }


    public async Task<CardListDto[]> SearchCards(CardSearchDto dto)
    {
        #region Requirements

        /*
           #1: Fetch N cards per Category for Yelp
           #2: Fetch N cards for AirBnB
           #3: Fetch N cards for GroupOn
           #4: Fetch unlimited cards for Own
           #5: Ignore already swiped cards by current user in last X hours
           #6: other than own card type current user can swipe only N cards in last X hours - 
               if user has already swiped y cards then next time the api should return N-y cards per card type
        */

        #endregion

        #region locals functions

        Expression<Func<Card, bool>> zipEql = x => x.ZipCode == dto.ZipCode;

        var wthOutLastDigit = dto.ZipCode.Length > 1 ? $"{dto.ZipCode[..^1]}%" : dto.ZipCode;
        Expression<Func<Card, bool>> zipLike = x => EF.Functions.Like(x.ZipCode, wthOutLastDigit);

        Expression<Func<SwipeHistory, bool>> CardNotSeenByCurrentUserInLast24Hours(CardType cardType)
        {
            return x =>
                x.Card.CardType == cardType && x.UserId == dto.UserId && x.SeenAtUtc > DateTime.Now.AddHours(-24) &&
                x.SeenAtUtc <= DateTime.Now;
        }

        #endregion

        var cardQuery =
            GetQueryable(
                    x => x.CreatedBy != dto.UserId /*&& x.CardType == CardType.Yelp || x.CardType == CardType.Own*/)
                .Where(x => !x.IsDeleted);

        var swipeHistories = AsNoTracking<SwipeHistory>();

        var cardQueryExcludingAlreadySwiped = GetCardQueryExcludingAlreadySwiped(dto, cardQuery, swipeHistories);

        #region Yelp

        var yelpAlreadySwipeCount =
            await swipeHistories.CountAsync(CardNotSeenByCurrentUserInLast24Hours(CardType.Yelp));

        var yelpCardQueryGroupedByCategory = await GetYelpCardQueryNItemPerCategory(yelpAlreadySwipeCount,
            cardQueryExcludingAlreadySwiped, zipEql, zipLike);

        #endregion

        /*#region GroupOn

        var groupAlreadySwipeCount =
            await swipeHistories.CountAsync(CardNotSeenByCurrentUserInLast24Hours(CardType.Groupon));
        var groupOnCardQuery =
            await GroupOnCardQuery(groupAlreadySwipeCount, cardQueryExcludingAlreadySwiped, zipEql, zipLike);

        #endregion

        #region AirBnB

        var airBnBAlreadySwipeCount =
            await swipeHistories.CountAsync(CardNotSeenByCurrentUserInLast24Hours(CardType.Airbnb));
        var airBnbOnCardQuery =
            await GetAirBnbOnCardQuery(airBnBAlreadySwipeCount, cardQueryExcludingAlreadySwiped, zipEql, zipLike);

        #endregion*/

        var ownCardQuery = GetOwnCardQuery(dto, cardQueryExcludingAlreadySwiped, zipEql);

        var cardIds = await
            yelpCardQueryGroupedByCategory.Select(x => x.Id)
                //.Union(groupOnCardQuery.Select(x => x.Id))
                //.Union(airBnbOnCardQuery.Select(x => x.Id))
                .Union(ownCardQuery.Select(x => x.Id))
                .ToArrayAsync();

        var cards = cardQuery.Where(x => cardIds.Contains(x.Id)).Include(x => x.SubCategories)
            .Include(x => x.CardPictures);

        var swpCounts =
            from swp in swipeHistories.Where(x => x.SwipeType == SwipeType.Right && cardIds.Contains(x.CardId))
            group swp by swp.CardId
            into g
            select new { CardId = g.Key, Count = g.Count() };

        var cardsWithCount =
            from c in cards
            from swp in swpCounts.Where(x => x.CardId == c.Id).DefaultIfEmpty()
            select new CardQuerySelector { Card = c, SwipeCount = swp.Count };


        return await cardsWithCount.ProjectTo<CardListDto>().ToArrayAsync();
    }

    private static IQueryable<Card> GetCardQueryExcludingAlreadySwiped(CardSearchDto dto, IQueryable<Card> cardQuery,
        IQueryable<SwipeHistory> swipeHistories)
    {
        return from cd in cardQuery
            join h in swipeHistories.Where(x => x.UserId == dto.UserId) on cd.Id equals h.CardId into j
            from h in j.DefaultIfEmpty()
            where h.CardId == null
            select new Card
            {
                Id = cd.Id, CardType = cd.CardType, ZipCode = cd.ZipCode, CategoryId = cd.CategoryId,
                CreatedBy = cd.CreatedBy
            };
    }

    private async Task<IQueryable<Card>> GetYelpCardQueryNItemPerCategory(int yelpAlreadySwipeCount,
        IQueryable<Card> cardQueryExcludingAlreadySwiped, Expression<Func<Card, bool>> zipEql,
        Expression<Func<Card, bool>> zipLike)
    {
        var categoryQuery = AsNoTracking<Category>();

        var categoryCount = await categoryQuery.CountAsync(); //TODO: can hard code count for perf improvement

        var yelpTotalRemainingCount = _appSetting.ItemCountPerProvider * categoryCount;

        if (yelpAlreadySwipeCount > 0)
            yelpTotalRemainingCount -= yelpAlreadySwipeCount;

        /*var yelpCardQuery = categoryQuery.Select(ct => ct.Id).SelectMany(categoryId =>
            cardQueryExcludingAlreadySwiped.Where(c => c.CategoryId == categoryId && c.CardType == CardType.Yelp));*/

        var yelpCardQuery = cardQueryExcludingAlreadySwiped.Where(c => c.CardType == CardType.Yelp);

        if (await yelpCardQuery.CountAsync(zipEql) >= _appSetting.ItemCountPerProvider * categoryCount)
        {
            yelpCardQuery = yelpCardQuery.Where(zipEql);
        }
        else if ((await yelpCardQuery.CountAsync(zipLike)) >= _appSetting.ItemCountPerProvider * categoryCount)
        {
            yelpCardQuery = yelpCardQuery.Where(zipLike);
        }
        else
        {
            yelpCardQuery = yelpCardQuery.Where(zipEql);
        }

        var yelpCardQueryGroupedByCategory = categoryQuery.Select(ct => ct.Id).SelectMany(categoryId =>
            yelpCardQuery.Where(c => c.CategoryId == categoryId)
                .OrderBy(x => Guid.NewGuid())
                .Take(_appSetting.ItemCountPerProvider)).Take(yelpTotalRemainingCount);
        return yelpCardQueryGroupedByCategory;
    }

    private IQueryable<Card> GetOwnCardQuery(CardSearchDto dto, IQueryable<Card> cardQueryExcludingAlreadySwiped,
        Expression<Func<Card, bool>> zipEql)
    {
        var currentUserId = dto.UserId; // (await _userContext.GetCurrentUserAsync(true))?.Id ?? 0;

        var blockedUserQuery = AsNoTracking<BlockedUser>().Where(x => x.BlockingUserId == currentUserId)
            .Select(x => x.BlockedUserId);

        const int privateChatCardId = -1;

        var ownCardQuery =
            cardQueryExcludingAlreadySwiped.Where(c =>
                c.CardType == CardType.Own && c.Id != privateChatCardId && !blockedUserQuery.Contains(c.CreatedBy));

        ownCardQuery = ownCardQuery.Where(zipEql);
        return ownCardQuery;
    }

    private async Task<IQueryable<Card>> GetAirBnbOnCardQuery(int airBnBAlreadySwipeCount,
        IQueryable<Card> cardQueryExcludingAlreadySwiped,
        Expression<Func<Card, bool>> zipEql, Expression<Func<Card, bool>> zipLike)
    {
        var airBnBCount = _appSetting.ItemCountPerProvider;
        if (airBnBAlreadySwipeCount > 0)
            airBnBCount = _appSetting.ItemCountPerProvider - airBnBAlreadySwipeCount;

        var airBnbOnCardQuery = cardQueryExcludingAlreadySwiped
            .Where(c => c.CardType == CardType.Airbnb);
        //.Select(cd => new Card { Id = cd.Id, ZipCode = cd.ZipCode });

        if (await airBnbOnCardQuery.CountAsync(zipEql) >= _appSetting.ItemCountPerProvider)
        {
            airBnbOnCardQuery = airBnbOnCardQuery.Where(zipEql);
        }
        else if (await airBnbOnCardQuery.CountAsync(zipLike) >= _appSetting.ItemCountPerProvider)
        {
            airBnbOnCardQuery = airBnbOnCardQuery.Where(zipLike);
        }
        else
        {
            airBnbOnCardQuery = airBnbOnCardQuery.Where(zipLike);
        }

        airBnbOnCardQuery = airBnbOnCardQuery.OrderBy(x => Guid.NewGuid())
            .Take(airBnBCount);
        return airBnbOnCardQuery;
    }

    private async Task<IQueryable<Card>> GroupOnCardQuery(int groupAlreadySwipeCount,
        IQueryable<Card> cardQueryExcludingAlreadySwiped,
        Expression<Func<Card, bool>> zipEql, Expression<Func<Card, bool>> zipLike)
    {
        var groupCount = _appSetting.ItemCountPerProvider;
        if (groupAlreadySwipeCount > 0)
            groupCount = _appSetting.ItemCountPerProvider - groupAlreadySwipeCount;

        var groupOnCardQuery = cardQueryExcludingAlreadySwiped
            .Where(c => c.CardType == CardType.Groupon);
        //.Select(cd => new Card { Id = cd.Id, ZipCode = cd.ZipCode });
        if (await groupOnCardQuery.CountAsync(zipEql) >= _appSetting.ItemCountPerProvider)
        {
            groupOnCardQuery = groupOnCardQuery.Where(zipEql);
        }

        else if (await groupOnCardQuery.CountAsync(zipLike) >= _appSetting.ItemCountPerProvider)
        {
            groupOnCardQuery = groupOnCardQuery.Where(zipLike);
        }
        else
        {
            groupOnCardQuery = groupOnCardQuery.Where(zipLike);
        }

        groupOnCardQuery = groupOnCardQuery.OrderBy(x => Guid.NewGuid())
            .Take(groupCount);
        return groupOnCardQuery;
    }

    public async Task<CardPictureDto[]> SearchCardsForPicture()
    {
        return await AsNoTracking.ProjectTo<CardPictureDto>().ToArrayAsync();
    }

    public async Task<Card[]> GetUserIdForExpireCard()
    {
        var swipeHistory = AsNoTracking<SwipeHistory>();
        var chatUser = AsNoTracking<Chat>();
        var cardQuery = from c in Table.Where(x =>
                x.CardType == CardType.Own
                && x.Id != -1
                && x.DateUtc <= DateTime.Now.AddHours(-48)
                && x.DateUtc <= DateTime.Now)
            from cu in chatUser.DefaultIfEmpty()
            where cu.CardId == c.Id
            from s in swipeHistory.DefaultIfEmpty()
            where cu.CardId == c.Id
            where cu == null && s == null
            select c;

        var cards = cardQuery.Distinct().ToArrayAsync();
        return await cards;
    }

    public void HardDeleteBulk(IEnumerable<Card> entities)
    {
        EntitySet.RemoveRange(entities);
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
    Task<Card[]> GetUserIdForExpireCard();
    void HardDeleteBulk(IEnumerable<Card> entities);
}