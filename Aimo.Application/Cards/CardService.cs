using Aimo.Core.Infrastructure;
using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.Categories;
using Aimo.Data.Infrastructure.Yelp;
using Aimo.Domain;
using Aimo.Domain.Cards;
using Aimo.Domain.Categories;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

namespace Aimo.Application.Cards;

internal partial class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly ISubCategoryRepository _subCategoryRepository;
    private readonly ICardPictureRepository _cardPictureRepository;
    private readonly CardDtoValidator _cardDtoValidator;
    private readonly IAppFileProvider _appFileProvider;
    private readonly IYelpHttpClient _yelpHttpClient;


    public CardService(
        ICardRepository cardRepository,
        ISubCategoryRepository subCategoryRepository,
        ICardPictureRepository cardPictureRepository,
        CardDtoValidator cardDtoValidator,
        IAppFileProvider appFileProvider,
        IYelpHttpClient yelpHttpClient
    )
    {
        _cardRepository = cardRepository;
        _cardDtoValidator = cardDtoValidator;
        _appFileProvider = appFileProvider;
        _yelpHttpClient = yelpHttpClient;
        _subCategoryRepository = subCategoryRepository;
        _cardPictureRepository = cardPictureRepository;
    }

    #region Utilities

    private async Task<string> UploadCardPictureAsync(string pictureUrl)
    {
        var image = new Base64Image(pictureUrl);

        var fileName = $"{Guid.NewGuid()}.{image.Extension}";
        var fileAbsPath = _appFileProvider.GetAbsolutePath(AppDefaults.CardPicturePath);
        _appFileProvider.CreateDirectory(fileAbsPath);
        await _appFileProvider.WriteAllBytesAsync(_appFileProvider.Combine(fileAbsPath, fileName), image.Bytes);
        var url = $"{AppDefaults.CardPicturePath}{fileName}";
        return url;
    }

    #endregion

    #region Methods

    public async ResultTask GetCardsAsync(CardSearchDto dto, CancellationToken ct = default)
    {
        #region method code

        dto.ThrowIfNull();

        var res = await SearchCardInDbAsync(dto);
        if (res.IsSucceeded)
            return res;

        var subcategories = await _subCategoryRepository.GetSubcategories<SubCategory>();

        if (subcategories!.IsNullOrEmpty())
            return res.Failure(ResultMessage.NotFound);

        //TODO: await Parallel.ForEachAsync()
        await FetchCardsFromYelp(dto, subcategories, ct);

        return await SearchCardInDbAsync(dto);

        #endregion
    }

    private async Task FetchCardsFromYelp(CardSearchDto dto, ICollection<SubCategory> subcategories,
        CancellationToken ct = default)
    {
        /*ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = 3
        };*/

        /*##foreach (var subCat in subcategories)*/
        await Parallel.ForEachAsync(subcategories, /*parallelOptions,*/ct, async (subCat, cnToken) =>
        {
            dto.Category = subCat.Category.Name;
            dto.SubCategory = subCat.Alias;
            dto.CategoryId = subCat.CategoryId;

            var searchResult = await _yelpHttpClient.SearchAsync(dto, cnToken);

            if (!searchResult.IsSucceeded)
            {
                //in Parallel.ForEachAsync return = continue
                //##continue; 
                return;
            }

            var cards = await SaveCardsInDb(subcategories, searchResult, subCat, cnToken);
            await GetCardPictureAndSaveAsync(cards);
        });
    }

    private async Task<Card[]> SaveCardsInDb(ICollection<SubCategory> subcategories,
        ListResult<YelpCardDto> searchResult, SubCategory subCat, CancellationToken ct)
    {
        #region local func

        Card CardResultSelector(YelpCardDto card, Category category)
        {
            var crd = card.Map<Card>();
            crd.Category = category;
            crd.SubCategories.AddRange(subcategories
                .Where(x => card.SubCategories.Select(s => s.Alias)
                    .Contains(x.Alias))
            );
            return crd;
        }

        #endregion

        var cards = new List<Card>();

        Parallel.ForEach(searchResult.Data, c => cards.Add(CardResultSelector(c, subCat.Category)));

        var newCards = await _cardRepository.AddCardsIfNotExists(cards);
        await _cardRepository.CommitAsync(ct: ct);
        return newCards;
    }

    private async ResultTask GetCardPictureAndSaveAsync(Card[] cards)
    {
        var cardPictureSearchDto = cards.Map<CardPictureDto[]>();
        foreach (var dto in cardPictureSearchDto)
        {
            var searchResult = await _yelpHttpClient.SearchCardPictureAsync(dto);
            if (!searchResult.IsSucceeded) continue;

            var cardPictures = searchResult.Data.Map<CardPicture[]>();
            await _cardPictureRepository.AddCardsPictureIfNotExists(cardPictures);
            await _cardPictureRepository.CommitAsync();
        }

        return Result.Create(cardPictureSearchDto);
    }


    private async ResultTask SearchCardInDbAsync(CardSearchDto dto)
    {
        var resp = await _cardRepository.SearchCards(dto);
        return resp.Any() ? Result.Create(resp).Success() : Result.Create().Failure(ResultMessage.ComeBack);
    }

    public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new CardDto());
        var entity = await _cardRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<CardDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async Task<Result<CardDto>> CreateAsync(CardDto dto)
    {
        var result = await _cardDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        if (dto.PictureUrl.IsNotEmpty())
            dto.PictureUrl = await UploadCardPictureAsync(dto.PictureUrl);
        try
        {
            var entity = dto.Map<Card>();
            await _cardRepository.AddAsync(entity);
            var affected = await _cardRepository.CommitAsync();
            
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }


    public async ResultTask UpdateAsync(CardDto dto)
    {
        var result = await _cardDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        try
        {
            var entity = await _cardRepository.GetByIdAsync(dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            if (dto.PictureUrl.IsNotEmpty())
            {
                var deletePicturePath = _appFileProvider.GetAbsolutePath(entity?.PictureUrl!);
                _appFileProvider.DeleteFile(deletePicturePath);
                dto.PictureUrl = await UploadCardPictureAsync(dto.PictureUrl);
            }

            dto.MapTo(entity);

            _cardRepository.Update(entity!);
            await _cardRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto)).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    public async ResultTask DeleteAsync(params int[] ids)
    {
        var result = Result.Create(false);
        try
        {
            var entity = await _cardRepository.FindBySpecAsync(new ByIdsSpec<Card>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);
            _cardRepository.RemoveBulk(entity);
            var affected = await _cardRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Exception(e);
        }
    }

    public async ResultTask ReportCardAsync(int cardId)
    {
        var card = await _cardRepository.FirstOrDefaultAsync(x => x.Id == cardId);
        if (card is not null && card.CardType == CardType.Own)
        {
            _cardRepository.Remove(card);
            await _cardRepository.CommitAsync();
            return Result.Create().Success();
        }

        return Result.Create().Failure(ResultMessage.NotFound);
    }

    public async Task<string> GetCardNameByIdAsync(int cardId)
    {
        return (await _cardRepository.FirstOrDefaultAsync(x => x.Id == cardId))?.Name ?? string.Empty;
    }

    #endregion
}

public partial interface ICardService
{
    ResultTask GetByIdAsync(int id);
    Task<Result<CardDto>> CreateAsync(CardDto dto);
    ResultTask UpdateAsync(CardDto viewDto);
    ResultTask DeleteAsync(params int[] ids);
    ResultTask GetCardsAsync(CardSearchDto dto, CancellationToken ct = default);
    ResultTask ReportCardAsync(int cardId);
    Task<string> GetCardNameByIdAsync(int dtoCardId);
}