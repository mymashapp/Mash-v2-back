using Aimo.Core.Infrastructure;
using Aimo.Core.Specifications;
using Aimo.Domain;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.Users
{
    public partial class UserUserPicturesService : IUserPicturesService
    {
        private readonly IRepository<UserPicture> _picturesRepository;
        private readonly IAppFileProvider _fileProvider;
        private readonly UserDtoValidator _userDtoValidator;
        private readonly UserPictureDtoValidator _userPictureDtoValidator;


        public UserUserPicturesService(IRepository<UserPicture> picturesRepository,
            IAppFileProvider fileProvider,
            UserDtoValidator userDtoValidator,
            UserPictureDtoValidator userPictureDtoValidator)
        {
            _picturesRepository = picturesRepository;
            _fileProvider = fileProvider;
            _userDtoValidator = userDtoValidator;
            _userPictureDtoValidator = userPictureDtoValidator;
        }

        public async ResultTask UpdateUserProfileOrCoverPicturesAsync(User entity, ICollection<UserPictureDto> pictures)
        {
            var result = await _userDtoValidator
                .ValidateResultAsync(new UserDto { UploadedPictures = pictures })
                .GetValidationResultFor(nameof(UserDto.UploadedPictures));

            if (!result.IsSucceeded)
                return result;


            if (!pictures.Any()) return result.Failure(ResultMessage.NullOrEmptyObject);

            //delete only profile and cover  not media
            var picturesToDelete = pictures.Where(x => x.PictureType != PictureType.Media).Select(p => p.PictureType)
                .Distinct();
            var pic = entity.Pictures.Where(x => picturesToDelete.Contains(x.PictureType)).ToArray();

            entity.Pictures.RemoveAll(x => picturesToDelete.Contains(x.PictureType));

            foreach (var picture in pictures)
            {
                var response = await UploadUserPictureAsync(new UserPictureDto
                {
                    UserId = entity.Id,
                    PictureUrl = picture.PictureUrl,
                    PictureType = picture.PictureType
                });

                if (response.IsSucceeded)
                {
                    await _picturesRepository.AddAsync(response.Data);
                }
                else
                {
                    result.Errors.GetOrAdd(
                        $"{nameof(UserPictureDto.PictureUrl)}_{nameof(picture.PictureType)}_{picture.PictureType}",
                        response.Message);
                    result.Failure(response.Message);
                }
            }

            await _picturesRepository.CommitAsync();

            if (pictures.IsNotEmpty())
                foreach (var p in pic)
                {
                    var deletePicturePath = _fileProvider.GetAbsolutePath(p.Url!);
                    _fileProvider.DeleteFile(deletePicturePath);
                }

            return result.Success();
        }

        public async Task<Result<UserPicture>> UploadUserPictureAsync(UserPictureDto dto)
        {
            try
            {
                var image = new Base64Image(dto.PictureUrl);
                var entity = dto.Map<UserPicture>();

                var fileName = $"{Guid.NewGuid()}.{image.Extension}";
                var fileAbsPath = _fileProvider.GetAbsolutePath(AppDefaults.UserPicturePath);
                _fileProvider.CreateDirectory(fileAbsPath);
                await _fileProvider.WriteAllBytesAsync(_fileProvider.Combine(fileAbsPath, fileName), image.Bytes);
                entity.Url = $"{AppDefaults.UserPicturePath}{fileName}";

                return Result.Create(entity).Success();
            }
            catch (Exception e)
            {
                return Result.Create<UserPicture>().Exception(e);
            }
        }

        public async ResultTask DeletePicturesAsync(int[] ids)
        {
            var result = Result.Create(false);
            try
            {
                var entity = await _picturesRepository.FindBySpecAsync(new ByIdsSpec<UserPicture>(ids));

                if (entity.IsNullOrEmpty())
                    return result.Failure(ResultMessage.NotFound);

                _picturesRepository.RemoveBulk(entity);
                var affected = await _picturesRepository.CommitAsync();

                if (entity.IsNotEmpty())
                    foreach (var p in entity)
                    {
                        var deletePicturePath = _fileProvider.GetAbsolutePath(p.Url!);
                        _fileProvider.DeleteFile(deletePicturePath);
                    }


                return result.SetData(affected > 0, affected).Success();
            }
            catch (Exception e)
            {
                return result.Exception(e);
            }
        }

        public async ResultTask AddUserMediaPicture(UserPictureDto dto)
        {
            var result = await _userPictureDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;

            try
            {
                var entity = await UploadUserPictureAsync(dto);
                await _picturesRepository.AddAsync(entity.Data);
                var affected = await _picturesRepository.CommitAsync();
                return result.SetData(entity.Data.MapTo(dto), affected).Success();
            }
            catch (Exception e)
            {
                return result.Exception(e);
            }
        }
    }

    public partial interface IUserPicturesService
    {
        Task<Result<UserPicture>> UploadUserPictureAsync(UserPictureDto dto);
        ResultTask UpdateUserProfileOrCoverPicturesAsync(User entity, ICollection<UserPictureDto> pictures);

        ResultTask DeletePicturesAsync(int[] dto);
        ResultTask AddUserMediaPicture(UserPictureDto dto);
    }
}