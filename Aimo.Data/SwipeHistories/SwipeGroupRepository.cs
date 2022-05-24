using Aimo.Data.Infrastructure;
using Aimo.Domain.Chats;
using Aimo.Domain.Data;
using Aimo.Domain.SwipeHistories;
using Aimo.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Aimo.Data.SwipeHistories;

internal partial class SwipeGroupRepository : EfRepository<SwipeGroup>, ISwipeGroupRepository
{
    public SwipeGroupRepository(IDataContext context) : base(context)
    {
    }


    public async Task<SwipeGroup[]> GetMatching(SwipeGroup swipeGroup)
    {
        
       // var swipeGroupInterest = swipeGroup.Interests.Count / 2;
        return await AsNoTracking.Where(x => (swipeGroup.GroupType ==GroupType.Three || x.Gender == swipeGroup.Gender)
                                             && x.GroupType == swipeGroup.GroupType
                                             && x.CurrentUserAge <= swipeGroup.AgeTo
                                                                   && x.CurrentUserAge >= swipeGroup.AgeFrom
                                                                   && x.CardId == swipeGroup.CardId
                                                                   && x.UserId != swipeGroup.UserId
                                             )
            //.Include(x=>x.Interests).Where(x=>x.Interests.Count(i=>swipeGroup.Interests.Any(si=>si.Id==i.Id))>=swipeGroupInterest)
            .Include(x=>x.User).ToArrayAsync();
    }

    public async Task<SwipeGroup[]> ReMatching(SwipeGroup swipeGroup, int[] alreadyMatchUser)
    {
        return await AsNoTracking.Where(x => (swipeGroup.GroupType ==GroupType.Three || x.Gender == swipeGroup.Gender)
                                             && x.GroupType == swipeGroup.GroupType
                                             && x.AgeTo >= swipeGroup.AgeTo 
                                             && x.AgeFrom <= swipeGroup.AgeFrom
                                             && x.CardId == swipeGroup.CardId
                                             && x.UserId != swipeGroup.UserId
                                             &&!alreadyMatchUser.Contains(x.UserId)
            )
            //.Include(x=>x.Interests).Where(x=>x.Interests.Count(i=>swipeGroup.Interests.Any(si=>si.Id==i.Id))>=swipeGroupInterest)
            .Include(x=>x.User).ToArrayAsync();
    }
}

public partial interface ISwipeGroupRepository : IRepository<SwipeGroup>
{
    Task<SwipeGroup[]> GetMatching(SwipeGroup swipeGroup);
    Task<SwipeGroup[]> ReMatching(SwipeGroup swipeGroup, int[] alreadyMatchUser);
}