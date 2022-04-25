using Aimo.Core.Infrastructure;
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


        public UserUserPicturesService(IRepository<UserPicture> picturesRepository, IAppFileProvider fileProvider,
            UserDtoValidator userDtoValidator)
        {
            _picturesRepository = picturesRepository;
            _fileProvider = fileProvider;
            _userDtoValidator = userDtoValidator;
        }

        public async ResultTask SaveUserPicturesAsync(User entity, ICollection<UserPictureDto> pictures)
        {
            var result = await _userDtoValidator
                .ValidateResultAsync(new UserDto { UploadedPictures = pictures })
                .GetValidationResultFor(nameof(UserDto.UploadedPictures));

            if (!result.IsSucceeded)
                return result;


            if (!pictures.Any()) return result.Failure(ResultMessage.NullOrEmptyObject);

            entity.Pictures.RemoveAll(x => pictures.Select(p => p.PictureType).Contains(x.PictureType));

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
                return Result.Create<UserPicture>().Failure(e.Message);
            }
        }
    }

    public partial interface IUserPicturesService
    {
        Task<Result<UserPicture>> UploadUserPictureAsync(UserPictureDto dto);
        ResultTask SaveUserPicturesAsync(User entity, ICollection<UserPictureDto> pictures);
    }
}