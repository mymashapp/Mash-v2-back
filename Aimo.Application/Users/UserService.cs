using Aimo.Core.Specifications;
using Aimo.Data.Users;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Interests;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.Users
{
    internal partial class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserDtoValidator _userDtoValidator;
        private readonly IRepository<Interest> _interestRepository;
        private readonly IUserPicturesService _userPicturesService;


        public UserService(
            IUserRepository userRepository,
            UserDtoValidator userDtoValidator,
            IRepository<Interest> interestRepository,
            IUserPicturesService userUserPicturesService)
        {
            _userRepository = userRepository;
            _userDtoValidator = userDtoValidator;
            _interestRepository = interestRepository;
            _userPicturesService = userUserPicturesService;
        }

        #region Utilities

        

        #endregion

        #region Methods

       

        public async ResultTask GetOrCreateUserByUidAsync(string uid)
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

        public async ResultTask UpdatePicturesAsync(UserPictureDto[] dto)
        {
            var result = Result.Create();

            if (!dto.Any())
                return result.Failure(ResultMessage.NullOrEmptyObject);

            try
            {
                var entity = await _userRepository.FirstOrDefaultAsync(x => x.Id == dto.First().UserId);
                if (entity is null)
                    return result.Failure(ResultMessage.NotFound);

                result = await _userPicturesService.SaveUserPicturesAsync(entity, dto);
                
                if (!result.IsSucceeded) return result;
            }
            catch (Exception e)
            {
                return result.Failure(e.Message);
            }

            return result.Success();
        }

        public async ResultTask UpdateAsync(UserDto dto)
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

                await _userPicturesService.SaveUserPicturesAsync(entity, dto.UploadedPictures);

                return result.SetData(entity.MapTo(dto)).Success();
            }
            catch (Exception e)
            {
                return result.Failure(e.Message);
            }
        }


        public async ResultTask DeleteAsync(params int[] ids)
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
        //ResultTask GetById(int id);
        ResultTask GetOrCreateUserByUidAsync(string uid);
        ResultTask UpdatePicturesAsync(UserPictureDto[] dto);
        ResultTask UpdateAsync(UserDto viewDto);
        ResultTask DeleteAsync(params int[] ids);
    }
}