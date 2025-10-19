using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IMatchPreferenceRepository
{
    Task<UserMatchPreference> GetUserMatchPreferenceAsync(Guid userId);
    Task AddAsync(UserMatchPreference entity);
    Task UpdateAsync(UserMatchPreference entity);
}
