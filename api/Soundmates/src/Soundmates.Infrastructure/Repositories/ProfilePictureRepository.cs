using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;

namespace Soundmates.Infrastructure.Repositories;

public class ProfilePictureRepository : IProfilePictureRepository
{
    public Task AddAsync(ProfilePicture entity)
    {
        throw new NotImplementedException();
    }

    public Task<ProfilePicture?> GetByIdAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Guid entityId)
    {
        throw new NotImplementedException();
    }
}
