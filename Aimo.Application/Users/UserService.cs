using Aimo.Core;
using Aimo.Core.Specifications;
using Aimo.Data.Users;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.Users
{
    internal partial class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDtoValidator _userDtoValidator;
        private readonly PictureDtoCollectionValidator _pictureDtoCollectionValidator;
        private readonly IRepository<Interest> _interestRepository;
        private readonly IPicturesService _picturesService;


        public UserService(
            IUserRepository userRepository,
            UserDtoValidator userDtoValidator,
            PictureDtoCollectionValidator pictureDtoCollectionValidator,
            IRepository<Interest> interestRepository,
            IPicturesService picturesService)
        {
            _userRepository = userRepository;
            _userDtoValidator = userDtoValidator;
            _interestRepository = interestRepository;
            _picturesService = picturesService;
            _pictureDtoCollectionValidator = pictureDtoCollectionValidator;
        }

        #region Utilities

        private async Task SaveUserPicturesAsync(User entity, PictureDto[] pictures)
        {
            if (pictures.Any())
            {
                entity.Pictures.Clear();
                foreach (var picture in pictures)
                {
                    await _picturesService.InsertPictureAsync(new PictureDto
                    {
                        UserId = entity.Id,
                        PictureUrl = picture.PictureUrl,
                        PictureType = picture.PictureType
                    });
                }
            }
        }

        #endregion
        #region Methods

        public async Task<IdNameDto[]> GetAllInterests()
        {
            return (await _interestRepository.FindAsync(x => true)).Map<IdNameDto[]>();
        }

        /*public async Task<Result<UserDto>> GetById(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);

            return entity is null
                ? Result.Create<UserDto>().Failure(ResultMessage.NotFound)
                : Result.Create(await PreparePicture(entity.Map<UserDto>())).Success();
        }*/

        public async Task<Result> GetOrCreateUserByUidAsync(string uid)
        {
            var entity = await _userRepository.FirstOrDefaultAsync(x => x.Uid == uid);

            if (entity is not null) return Result.Create(entity.Map<UserDto>()).Success();

            var user = new User
            {
                Uid = uid,
                Bio = string.Empty,
                DateOfBirth = default,
                Name = string.Empty,
                Email = string.Empty,
                IsActive = true,
                IsNew = true
            };

            await _userRepository.AddAsync(user);

            await _userRepository.CommitAsync();

            var dto = user.Map<UserDto>();
            return Result.Create(dto).Success();
        }

        public async Task<Result> UpdatePictures(PictureDto[] dto)
        {
            
            var result = Result.Create(0);

            if (!dto.Any())
                return result.Failure(ResultMessage.NullObject);

            try
            {
                var entity = await _userRepository.FirstOrDefaultAsync(x => x.Id == dto.First().UserId);
                if (entity is null)
                    return result.Failure(ResultMessage.NotFound);

                await SaveUserPicturesAsync(entity, dto);
            }
            catch (Exception e)
            {
                return result.Failure(e.Message);
            }
            
            return result.Success();
        }

        public async Task<Result> Update(UserDto dto)
        {
            var result = await _userDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;

            try
            {
                var entity = await _userRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
                if (entity is null)
                    return result.Failure(ResultMessage.NotFound);

                if (dto.SelectedInterestIds.Any())
                {
                    entity.Interests.Clear();
                    var interests = await _interestRepository.FindAsync(x => dto.SelectedInterestIds.Contains(x.Id));
                    entity.Interests.AddRange(interests);
                }

                dto.MapTo(entity);
                entity.IsNew = false;

                _userRepository.Update(entity);
                await _userRepository.CommitAsync();

                await SaveUserPicturesAsync(entity, dto.UploadedPictures);

                return result.SetData(entity.MapTo(dto)).Success();
            }
            catch (Exception e)
            {
                return result.Failure(e.Message);
            }
        }

      

        public async Task<Result<bool>> Delete(params int[] ids)
        {
            var result = Result.Create(false);
            try
            {
                var entity = await _userRepository.FindBySpecAsync(new ByIdsSpec<User>(ids));

                if (!entity.IsNullOrEmpty())
                    return result.Failure(ResultMessage.NotFound);
                _userRepository.RemoveBulk(entity);
                var affected = await _userRepository.CommitAsync();
                return result.SetData(affected > 0, affected).Success();
            }
            catch (Exception e)
            {
                return result.Failure(e.Message);
            }
        }

        #endregion
    }

    public partial interface IUserService
    {
        //Task<Result<UserDto>> GetById(int id);

        Task<Result> GetOrCreateUserByUidAsync(string uid);
        Task<Result> UpdatePictures(PictureDto[] dto);
        Task<Result> Update(UserDto viewDto);
        Task<Result<bool>> Delete(params int[] ids);
        Task<IdNameDto[]> GetAllInterests();
    }
}