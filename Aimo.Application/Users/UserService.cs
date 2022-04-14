using Aimo.Core;
using Aimo.Core.Specifications;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;

namespace Aimo.Application.Users
{
    internal partial class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserDtoValidator _userDtoValidator;
        private readonly IRepository<Picture> _pictureRepository;

        public UserService(IRepository<User> userRepository, UserDtoValidator userDtoValidator,
            IRepository<Picture> pictureRepository)
        {
            _userRepository = userRepository;
            _userDtoValidator = userDtoValidator;
            _pictureRepository = pictureRepository;
        }

        #region Methods

        protected virtual async Task<UserDto> PreparePicture(UserDto dto)
        {
            var profilePicture = await _pictureRepository.FirstOrDefaultAsync(x => x.Id == dto.ProfilePictureId);

            if (profilePicture is not null)
                dto.ProfilePictureUrl = "data:image/png;base64," + Convert.ToBase64String(profilePicture.Binary);

            var userPhotos = new List<UserPhotoDto>();
            var userPicIds = dto.UserPhotos.Select(p => p.Id);
            var userPics = await _pictureRepository.Find(x => userPicIds.Contains(x.Id));
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

        public async Task<Result<UserDto>> GetById(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);

            return entity is null
                ? Result.Create<UserDto>().Failure(ResultMessage.NotFound)
                : Result.Create(await PreparePicture(entity.Map<UserDto>())).Success();
        }

        public async Task<Result<UserDto>> GetOrCreateUserByUidAsync(string uid)
        {
            var entity = await _userRepository.FirstOrDefaultAsync(x => x.Uid == uid);

            if (entity is not null) return Result.Create(await PreparePicture(entity.Map<UserDto>())).Success();

            var user = new User
            {
                Uid = uid, 
                Bio = string.Empty,
                Gender = string.Empty, 
                DateOfBirth = default, 
                Name = string.Empty,
                Email = string.Empty
            };

            await _userRepository.AddAsync(user);

            await _userRepository.CommitAsync();

            var dto = user.MapTo(new UserDto { IsNew = true });
            return Result.Create(dto).Success();
        }

        public async Task<Result<UserDto>> Create(UserDto dto)
        {
            var result = await _userDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;
            try
            {
                var entity = dto.Map<User>();

                await _userRepository.AddAsync(entity);
                var affected = await _userRepository.CommitAsync();
                dto.IsNew = true;
                return result.SetData(await PreparePicture(entity.MapTo(dto)), affected).Success();
            }
            catch (Exception e)
            {
                return result.Failure(e.Message);
            }
        }

        public async Task<Result<UserDto>> Update(UserDto dto)
        {
            var result = await _userDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;

            try
            {
                var entity = dto.Map<User>();

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
        Task<Result<UserDto>> GetById(int id);
        Task<Result<UserDto>> GetOrCreateUserByUidAsync(string uid);
        Task<Result<UserDto>> Create(UserDto dto);
        Task<Result<UserDto>> Update(UserDto dto);
        Task<Result<bool>> Delete(params int[] ids);
    }
}