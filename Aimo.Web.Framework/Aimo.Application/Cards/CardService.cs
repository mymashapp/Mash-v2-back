using Aimo.Core.Infrastructure;
using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.Categories;
using Aimo.Data.Infrastructure.Yelp;
using Aimo.Domain;
using Aimo.Domain.Cards;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using SubCategory = Aimo.Domain.Categories.SubCategory;

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

    #endregion

    #region Methods

    public async ResultTask GetCardsAsync(CardSearchDto dto, CancellationToken ct = default)
    {
        #region method code

        dto.ThrowIfNull();

        //TODO: search in db first and return matching result
        var res = await SearchCardInDbAsync(dto);
        if (res.IsSucceeded)
            return res;

        var subcategories = await _subCategoryRepository.GetSubcategories<SubCategory>();

        if (subcategories!.IsNullOrEmpty())
            return res.Failure(ResultMessage.NotFound);

        //TODO: await Parallel.ForEachAsync()
        foreach (var subCat in subcategories)
        {
            dto.Category = subCat.Category.Name;
            dto.SubCategory = subCat.Alias;
            dto.CategoryId = subCat.CategoryId;

            var searchResult = await _yelpHttpClient.SearchAsync(dto, ct);

            if (!searchResult.IsSucceeded) continue;

            var cards = searchResult.Data.Select(c => CardResultSelector(c, subCat)).ToArray();
            var cardForSearchPicture = await _cardRepository.AddCardsIfNotExists(cards);
            await _cardRepository.CommitAsync(ct: ct);
            await SearchCardPictureAsync(cardForSearchPicture);
        }


        return await SearchCardInDbAsync(dto);

        #endregion

        #region local func

        Card CardResultSelector(YelpCardDto c, SubCategory subCategory)
        {
            var crd = c.Map<Card>();
            crd.Category = subCategory.Category;
            crd.SubCategories.AddRange(subcategories
                .Where(x => c.SubCategories.Select(s => s.Alias)
                    .Contains(x.Alias))
            );
            return crd;
        }

        #endregion
    }

    private async ResultTask SearchCardPictureAsync(Card[] cards)
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
        return resp.Any() ? Result.Create(resp).Success() : Result.Create().Failure(ResultMessage.NotFound);
    }

    public async ResultTask GetByIdAsync(int id)
    {
        var result = Result.Create(new CardDto());
        var entity = await _cardRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<CardDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async ResultTask CreateAsync(CardDto dto)
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
                _appFileProvider.DeleteFile(entity?.PictureUrl!);
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

    public async Task<string> UploadCardPictureAsync(string pictureUrl)
    {
        var image = new Base64Image(pictureUrl);

        var fileName = $"{Guid.NewGuid()}.{image.Extension}";
        var fileAbsPath = _appFileProvider.GetAbsolutePath(AppDefaults.CardPicturePath);
        _appFileProvider.CreateDirectory(fileAbsPath);
        await _appFileProvider.WriteAllBytesAsync(_appFileProvider.Combine(fileAbsPath, fileName), image.Bytes);
        var url = $"{AppDefaults.CardPicturePath}{fileName}";
        return url;
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

    #endregion
}

public partial interface ICardService
{
    ResultTask GetByIdAsync(int id);
    ResultTask CreateAsync(CardDto dto);
    ResultTask UpdateAsync(CardDto viewDto);
    ResultTask DeleteAsync(params int[] ids);
    ResultTask GetCardsAsync(CardSearchDto dto, CancellationToken ct = default);
}