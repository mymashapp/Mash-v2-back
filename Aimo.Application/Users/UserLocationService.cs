using Aimo.Core.Infrastructure;
using Aimo.Core.Specifications;
using Aimo.Data.Users;
using Aimo.Domain;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Application.Users
{
    public partial class UserLocationService : IUserLocationService
    {
        private readonly IUserLocationRepository _userLocationRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserLocationDtoValidator _userLocationDtoValidator;


        public UserLocationService(IUserLocationRepository userLocationRepository,
            IUserRepository userRepository,
            UserLocationDtoValidator userLocationDtoValidator)
        {
            _userLocationRepository = userLocationRepository;
            _userRepository = userRepository;
            _userLocationDtoValidator = userLocationDtoValidator;
        }

        public async ResultTask CreateAsync(UserLocationDto dto)
        {
            var result = await _userLocationDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;

            try
            {
                var entity = dto.Map<UserLocation>();
                await _userLocationRepository.AddAsync(entity);
                var affected = await _userLocationRepository.CommitAsync();
                return result.SetData(entity.MapTo(dto), affected).Success();
            }
            catch (Exception e)
            {
                return result.Exception(e);
            }
        }

        public async ResultTask GetUserInXMile(UserLocationDto dto)
        {
            var result = Result.Create(new List<UserLocationDto>());
            var entities = await _userLocationRepository.GetUserInXMile(dto);
            
            var locationDtos = entities.Map<List<UserLocationDto>>();
            if (entities!.IsNotEmpty())
            {
                foreach (var item in locationDtos)
                {
                    var user = await _userRepository.GetUserWithProfilePicture(item.UserId);
                    item.UserName = user.Name;
                    item.UserImage = user.PictureUrl;
                }
                
                return result.SetData(locationDtos).Success();
            }

            return result.Failure(ResultMessage.NotFound);
        }

        public async ResultTask UpdateAsync(UserLocationDto dto)
        {
            var result = await _userLocationDtoValidator.ValidateResultAsync(dto);
            if (!result.IsSucceeded)
                return result;
            try
            {
                var entity = await _userLocationRepository.FirstOrDefaultAsync(x => x.UserId == dto.UserId);
                if (entity is null)
                    return await CreateAsync(dto);
                // result.Failure(ResultMessage.NotFound);

                dto.Id = entity.Id;
                dto.MapTo(entity);

                _userLocationRepository.Update(entity);
                await _userLocationRepository.CommitAsync();
                return result.SetData(entity.MapTo(dto)).Success();
            }
            catch (Exception e)
            {
                return result.Exception(e);
            }
        }
    }

    public interface IUserLocationService
    {
        ResultTask UpdateAsync(UserLocationDto dto);
        ResultTask CreateAsync(UserLocationDto dto);
        ResultTask GetUserInXMile(UserLocationDto dto);
    }
}