using Aimo.Core;
using Aimo.Core.Specifications;
using Aimo.Data.Users;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

namespace Aimo.Application.Users
{
    internal partial class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDtoValidator _userDtoValidator;
        private readonly IRepository<Picture> _pictureRepository;
        private readonly IRepository<Interest> _interestRepository;

        public UserService(
            IUserRepository userRepository,
            UserDtoValidator userDtoValidator,
            IRepository<Picture> pictureRepository,
            IRepository<Interest> interestRepository)
        {
            _userRepository = userRepository;
            _userDtoValidator = userDtoValidator;
            _pictureRepository = pictureRepository;
            _interestRepository = interestRepository;
        }

        #region Methods

        protected virtual async Task<UserViewDto> PreparePicture(UserViewDto dto)
        {
            var profilePicture = await _pictureRepository.FirstOrDefaultAsync(x => x.Id == dto.ProfilePictureId);

            if (profilePicture is not null)
                dto.ProfilePictureUrl = "data:image/png;base64," + Convert.ToBase64String(profilePicture.Binary);

            var userPhotos = new List<UserPhotoViewDto>();
            var userPicIds = dto.UserPhotos.Select(p => p.Id);
            var userPics = await _pictureRepository.FindAsync(x => userPicIds.Contains(x.Id));
            foreach (var item in dto.UserPhotos)
            {
                var photo = userPics.FirstOrDefault(x => x.Id == dto.Id);
                if (photo?.Binary is null) continue;

                item.PictureUrl = "data:image/png;base64," + Convert.ToBase64String(photo.Binary);
                userPhotos.Add(item);
            }

            dto.UserPhotos = userPhotos;
            return dto;
        }

        public async Task<IdNameDto[]> GetAllInterests()
        {
            return (await _interestRepository.FindAsync(x => true)).Map<IdNameDto[]>();
        }

        public async Task<Result<UserViewDto>> GetById(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);

            return entity is null
                ? Result.Create<UserViewDto>().Failure(ResultMessage.NotFound)
                : Result.Create(await PreparePicture(entity.Map<UserViewDto>())).Success();
        }

        public async Task<Result<UserViewDto>> GetOrCreateUserByUidAsync(string uid)
        {
            var entity = await _userRepository.FirstOrDefaultAsync(x => x.Uid == uid);

            if (entity is not null) return Result.Create(await PreparePicture(entity.Map<UserViewDto>())).Success();

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

            var dto = user.Map<UserViewDto>();
            return Result.Create(dto).Success();
        }

        public async Task<Result<UserSaveDto>> Update(UserSaveDto dto)
        {
            var result = await _userDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;

            try
            {
                var entity = dto.Map<User>();
                var interests = await _interestRepository.FindAsync(x => dto.InterestIds.Contains(x.Id));
                entity.Interests.AddRange(interests);

                _userRepository.Update(entity);
                var affected = await _userRepository.CommitAsync();

                return result.SetData(entity.MapTo(dto), affected).Success();
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
        Task<Result<UserViewDto>> GetById(int id);

        Task<Result<UserViewDto>> GetOrCreateUserByUidAsync(string uid);

        Task<Result<UserSaveDto>> Update(UserSaveDto viewDto);
        Task<Result<bool>> Delete(params int[] ids);
        Task<IdNameDto[]> GetAllInterests();
    }
}