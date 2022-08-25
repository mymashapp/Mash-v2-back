using Aimo.Application.Chats;
using Aimo.Core.Specifications;
using Aimo.Data.Cards;
using Aimo.Data.Chats;
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
        private readonly IBlockedUserRepository _blockedUserRepository;
        private readonly IChatUserRepository _chatUserRepository;
        private readonly IUserContext _userContext;
        private readonly IChatMessageService _chatMessageService;
        private readonly ICardRepository _cardRepository;

        public UserService(
            IUserRepository userRepository,
            UserDtoValidator userDtoValidator,
            IRepository<Interest> interestRepository,
            IUserPicturesService userUserPicturesService,
            IBlockedUserRepository blockedUserRepository, IChatUserRepository chatUserRepository,
            IUserContext userContext, IChatMessageService chatMessageService, ICardRepository cardRepository)
        {
            _userRepository = userRepository;
            _userDtoValidator = userDtoValidator;
            _interestRepository = interestRepository;
            _userPicturesService = userUserPicturesService;
            _blockedUserRepository = blockedUserRepository;
            _chatUserRepository = chatUserRepository;
            _userContext = userContext;
            _chatMessageService = chatMessageService;
            _cardRepository = cardRepository;
        }

        #region Utilities

        #endregion

        #region Methods

        public async Task<Result<UserDto>> GetOrCreateUserByUidAsync(string uid)
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

                result = await _userPicturesService.UpdateUserProfileOrCoverPicturesAsync(entity, dto);

                if (!result.IsSucceeded) return result;
            }
            catch (Exception e)
            {
                return result.Exception(e);
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

                await _userPicturesService.UpdateUserProfileOrCoverPicturesAsync(entity, dto.UploadedPictures);

                return result.SetData(entity.MapTo(dto)).Success();
            }
            catch (Exception e)
            {
                return result.Exception(e);
            }
        }


        public async ResultTask DeleteAsync(int id)
        {
            var result = Result.Create(false);
            try
            {
                var chatUser = await _chatUserRepository.GetChatUserForDelete(id);
                //var entity = await _userRepository.FindBySpecAsync(new ByIdsSpec<User>(ids));
                var entity = await _userRepository.FirstOrDefaultForDeleteAsync(x => x.Id == id);

                if (entity is null)
                    return result.Failure(ResultMessage.NotFound);
                _chatUserRepository.RemoveBulk(chatUser);
                await _chatUserRepository.CommitAsync();
                _userRepository.Remove(entity);
                var userCards = await _cardRepository.FindAsync(x => x.CreatedBy == id);
                _cardRepository.HardDeleteBulk(userCards);
                var affected = await _userRepository.CommitAsync();
                return result.SetData(affected > 0, affected).Success();
            }
            catch (Exception e)
            {
                return result.Exception(e);
            }
        }

        public async ResultTask ToggleUserLocationAsync(int userId)
        {
            var result = Result.Create();
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null)
                return result;

            user.UserLocationEnabled = !user.UserLocationEnabled;
            _userRepository.Update(user);
            await _userRepository.CommitAsync();
            return result.Success();
        }

        public async ResultTask BlockUserAsync(int blockUserId)
        {
            var currentUser = await _userContext.GetCurrentUserAsync(true);
            var blockedUser = await _blockedUserRepository.FirstOrDefaultAsync(x =>
                x.BlockingUserId == currentUser!.Id && x.BlockedUserId == blockUserId);
            if (blockedUser is null)
            {
                var blockUser = new BlockedUser
                {
                    BlockedUserId = blockUserId,
                    BlockingUserId = currentUser!.Id
                };
                await _blockedUserRepository.AddAsync(blockUser);
                await _blockedUserRepository.CommitAsync();
            }

            var result = await _chatMessageService.RemoveChatHistoryOfBlockUser(currentUser.Id, blockUserId);
            return result.Success();
        }

        public async ResultTask UnBlockUserAsync(int unBlockUserId)
        {
            var currentUser = await _userContext.GetCurrentUserAsync(true);
            var blockedUser = await _blockedUserRepository.FirstOrDefaultAsync(x =>
                x.BlockedUserId == unBlockUserId && x.BlockingUserId == currentUser!.Id);
            if (blockedUser is not null)
                _blockedUserRepository.Remove(blockedUser);

            await _blockedUserRepository.CommitAsync();
            return Result.Create().Success();
        }

        public async ResultTask GetAllBlockedUsersForCurrentUser()
        {
            var currentUser = await _userContext.GetCurrentUserAsync(true);
            var blockedUsers = await _blockedUserRepository.FindAsync(x => x.BlockingUserId == currentUser!.Id);
            if (blockedUsers is not null)
            {
                var users = await _userRepository.FindAsync(x =>
                    blockedUsers.Select(x => x.BlockedUserId).Contains(x.Id));
                if (users is not null)
                    return Result.Create(await _userRepository.GetUserProfile(users.Map<List<UserDto>>())).Success();
            }

            return Result.Create().Failure(ResultMessage.NotFound);
        }

        public async ResultTask GetByIdAsync(int id)
        {
            var result = Result.Create(new UserDto());
            var entity = await _userRepository.GetByIdAsync(id);
            return entity is not null
                ? result.SetData(entity.Map<UserDto>()).Success()
                : result.Failure(ResultMessage.NotFound);
        }

        public async ResultTask SetUserFCMTokenAsync(string uid, string FCMtoken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Uid == uid);
            var result = Result.Create();
            if (user is null)
                return result.Failure(ResultMessage.NotFound);

            user.FCMToken = FCMtoken;
            _userRepository.Update(user);
            await _userRepository.CommitAsync();
            return result.Success();
        }

        #endregion
    }

    public partial interface IUserService
    {
        ResultTask GetByIdAsync(int id);
        Task<Result<UserDto>> GetOrCreateUserByUidAsync(string uid);
        ResultTask UpdatePicturesAsync(UserPictureDto[] dto);
        ResultTask UpdateAsync(UserDto viewDto);
        ResultTask DeleteAsync(int id);
        ResultTask ToggleUserLocationAsync(int userId);
        ResultTask BlockUserAsync(int blockUserId);
        ResultTask UnBlockUserAsync(int unBlockUserId);
        ResultTask GetAllBlockedUsersForCurrentUser();
        ResultTask SetUserFCMTokenAsync(string uuid, string FCMtoken);
    }
}