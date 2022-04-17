using Aimo.Core.Infrastructure;
using Aimo.Core.Specifications;
using Aimo.Data.Card;
using Aimo.Domain;
using Aimo.Domain.Card;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.Card;

internal partial class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly CardDtoValidator _cardDtoValidator;
    private readonly IAppFileProvider _appFileProvider;


    public CardService(
        ICardRepository cardRepository,
        CardDtoValidator cardDtoValidator, IAppFileProvider appFileProvider)
    {
        _cardRepository = cardRepository;
        _cardDtoValidator = cardDtoValidator;
        _appFileProvider = appFileProvider;
    }

    #region Utilities

    #endregion

    #region Methods

    public async ResultTask GetById(int id)
    {
        var result = Result.Create(new CardDto());
        var entity = await _cardRepository.GetByIdAsync(id);
        return entity is not null
            ? result.SetData(entity.Map<CardDto>()).Success()
            : result.Failure(ResultMessage.NotFound);
    }

    public async ResultTask Create(CardDto dto)
    {
        var result = await _cardDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;

        dto.PictureUrl = await UploadCardPictureAsync(dto.PictureUrl);
        try
        {
            var entity = dto.Map<Domain.Card.Card>();
            await _cardRepository.AddAsync(entity);
            var affected = await _cardRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto), affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    public async ResultTask Update(CardDto dto)
    {
        var result = await _cardDtoValidator.ValidateResultAsync(dto);
        if (!result.IsSucceeded)
            return result;
        
        dto.PictureUrl = await UploadCardPictureAsync(dto.PictureUrl);

        try
        {
            var entity = await _cardRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity is null)
                return result.Failure(ResultMessage.NotFound);

            dto.MapTo(entity);

            _cardRepository.Update(entity);
            await _cardRepository.CommitAsync();
            return result.SetData(entity.MapTo(dto)).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
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

    public async Task<Result<bool>> Delete(params int[] ids)
    {
        var result = Result.Create(false);
        try
        {
            var entity = await _cardRepository.FindBySpecAsync(new ByIdsSpec<Domain.Card.Card>(ids));

            if (!entity.IsNullOrEmpty())
                return result.Failure(ResultMessage.NotFound);
            _cardRepository.RemoveBulk(entity);
            var affected = await _cardRepository.CommitAsync();
            return result.SetData(affected > 0, affected).Success();
        }
        catch (Exception e)
        {
            return result.Failure(e.Message);
        }
    }

    #endregion
}

public partial interface ICardService
{
    ResultTask GetById(int id);
    ResultTask Create(CardDto dto);
    ResultTask Update(CardDto viewDto);
    Task<Result<bool>> Delete(params int[] ids);
}
