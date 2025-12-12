using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMatchPreferenceRepository
{
    Task<UserMatchPreference?> GetUserMatchPreferenceAsync(Guid userId);
    Task AddUpdateAsync(UserMatchPreference entity, IList<Guid>? filterTagsIds = null);
}
