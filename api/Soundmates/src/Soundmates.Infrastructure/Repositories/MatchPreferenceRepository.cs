using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class MatchPreferenceRepository : IMatchPreferenceRepository
{
    public Task AddAsync(UserMatchPreference entity)
    {
        throw new NotImplementedException();
    }

    public Task<UserMatchPreference> GetUserMatchPreferenceAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UserMatchPreference entity, IList<Guid> FilterTagsIds)
    {
        throw new NotImplementedException();
    }
}
