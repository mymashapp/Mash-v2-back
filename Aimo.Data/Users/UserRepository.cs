using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.Users;

internal class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(IDataContext context) : base(context)
    {
    }

    public override async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>>? predicate = null,
        params Expression<Func<User, object>>[] include)
    {
        return await base.FirstOrDefaultAsync(predicate, user => user.Pictures, user => user.Interests);
    }

    public async Task<ChatMemberDto> GetUserWithProfilePicture(int userId)
    {
        var result = (await FirstOrDefaultAsync(x => x.Id == userId)).MapTo(new ChatMemberDto());

        var userPicture = await AsNoTracking<UserPicture>()
            .Where(x => x.UserId == userId && x.PictureType == PictureType.ProfilePicture).ToArrayAsync();

        result.PictureUrl = userPicture?.FirstOrDefault(x => x.UserId == result.Id)?.Url ?? string.Empty;
        return result;
    }

    public async Task<List<UserDto>> GetUserProfile(List<UserDto> userDtos)
    {
        foreach (var dto in userDtos)
        {
            var userPicture = await AsNoTracking<UserPicture>()
            .FirstOrDefaultAsync(x => x.UserId == dto.Id && x.PictureType == PictureType.ProfilePicture);

            if (userPicture is not null)
                dto.Pictures.Add(userPicture.Map<UserPictureDto>());
        }

        return userDtos;
    }

    public async Task<User?> FirstOrDefaultForDeleteAsync(Expression<Func<User, bool>>? predicate = null, params Expression<Func<User, object>>[] include)
    {
        return await base.FirstOrDefaultAsync(predicate, user => user.Pictures, user => user.Interests,user=>user.UserLocations);
    }
}

public partial interface IUserRepository : IRepository<User>
{
    Task<ChatMemberDto> GetUserWithProfilePicture(int userId);
    Task<List<UserDto>> GetUserProfile(List<UserDto> userDtos);
    Task<User?> FirstOrDefaultForDeleteAsync(Expression<Func<User, bool>>? predicate = null,
        params Expression<Func<User, object>>[] include);
}