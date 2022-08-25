using System.Linq.Expressions;
using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.Infrastructure;
using Aimo.Domain.Users;
using Aimo.Domain.Users.Entities;

namespace Aimo.Data.Users;

internal class UserLocationRepository : EfRepository<UserLocation>, IUserLocationRepository
{
    private readonly IUserContext _userContext;
    private readonly IBlockedUserRepository _blockedUserRepository;

    public UserLocationRepository(IDataContext context, IUserContext userContext
        , IBlockedUserRepository blockedUserRepository) : base(context)
    {
        _userContext = userContext;
        _blockedUserRepository = blockedUserRepository;
    }


    public async Task<UserLocation[]> GetUserInXMile(UserLocationDto dto)
    {
        var currentUser = await _userContext.GetCurrentUserAsync(true);
        var sql = $@";with RestroomLocationsWithDistance as
                 (
                      select
                        [UserLocation].[Id],[UserLocation].[UserId],[UserLocation].[Latitude],[UserLocation].[Longitude],
                        ( 3959 * acos( cos( radians({dto.Latitude}) ) 
                               * cos( radians( [Latitude] ) )
                               * cos( radians( [Longitude] )
                               - radians({dto.Longitude}) ) + sin( radians({dto.Latitude}) )
                               * sin( radians( [Latitude] ) ) ) ) As [Distance]
                      FROM [dbo].[UserLocation]
                      INNER JOIN [User] ON [UserLocation].[UserId]=[User].[Id]
					  where [user].[UserLocationEnabled] = 1
                 )
                 select
                    [Id],[UserId],[Latitude],[Longitude],[Distance]
                 from RestroomLocationsWithDistance
                 where Distance <= {dto.Distance} ";

        var query = await FromSqlRaw(sql);

        var blockedUser =
            (await _blockedUserRepository.FindAsync(x => currentUser != null && x.BlockingUserId == currentUser.Id));
        var blockingUserIds = blockedUser.Select(x => x.BlockingUserId).ToArray();
        var blockedUserIds = blockedUser.Select(x => x.BlockedUserId).ToArray();
        if (blockedUser.Any())
            query = query.Where(x => !blockedUserIds.Contains(x.UserId) && !blockingUserIds.Contains(x.UserId));
        return query.Distinct().ToArray();
    }
}

public interface IUserLocationRepository : IRepository<UserLocation>
{
    Task<UserLocation[]> GetUserInXMile(UserLocationDto dto);
}