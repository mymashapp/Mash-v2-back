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
        private readonly IRepository<Picture> _pictureRepositery;

        public UserService(IRepository<User> userRepository, UserDtoValidator userDtoValidator,
            IRepository<Picture> pictureRepositery)
        {
            _userRepository = userRepository;
            _userDtoValidator = userDtoValidator;
            _pictureRepositery = pictureRepositery;
        }

        #region Methods
        protected async Task<UserDto> PreparePicture(UserDto dto)
        {
            var profilePicture = await _pictureRepositery.FirstOrDefaultAsync(x => x.Id == dto.ProfilePictureId);
            if (profilePicture is not null)
                dto.ProfilePictureUrl = "data:image/png;base64," + Convert.ToBase64String(profilePicture.Binary);

            var userPhotos = new List<UserPhotoDto>();
            foreach (var item in dto.UserPhotos)
            {
                var photo = await _pictureRepositery.FirstOrDefaultAsync(x => x.Id == dto.ProfilePictureId);
                if (profilePicture is not null)
                {
                    item.PictureUrl = "data:image/png;base64," + Convert.ToBase64String(photo.Binary);
                    userPhotos.Add(item);
                }
            }
            dto.UserPhotos = userPhotos;
            return dto;
        }
        public async Task<Result<UserDto>> GetById(int id)
        {
            var result = Result.Create(new UserDto());
            var entity = await _userRepository.GetByIdAsync(id);
            if (entity is not null)
            {
                result.SetData(entity.Map<UserDto>()).Success();
                result.Data = await PreparePicture(result.Data);
                return result;
            }
            return result.Failure(ResultMessage.NotFound);
        }

        public async Task<Result<UserDto>> GetOrCreateUserByUidAsync(string uid = "")
        {
            var result = Result.Create(new UserDto());
            var entity = await _userRepository.FirstOrDefaultAsync(x => x.Uid == uid);
            if (entity is null)
            {
                var user = new User() { Uid = uid };
                await _userRepository.AddAsync(user);
                var affected = await _userRepository.CommitAsync();
                result.SetData(user.Map<UserDto>(), affected).Success();
                result.Data.IsNew = true;
                return result;
            }
            result.SetData(entity.Map<UserDto>()).Success();
            result.Data = await PreparePicture(result.Data);
            return result;

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
                result.SetData(entity.MapTo(dto), affected).Success();
                result.Data = await PreparePicture(result.Data);
                return result;
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
        Task<Result<UserDto>> GetOrCreateUserByUidAsync(string uid = "");
        Task<Result<UserDto>> Create(UserDto dto);
        Task<Result<UserDto>> Update(UserDto dto);
        Task<Result<bool>> Delete(params int[] ids);
    }
}
