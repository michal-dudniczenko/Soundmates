using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IProfilePictureRepository
{
    Task AddAsync(ProfilePicture entity);
    Task RemoveAsync(Guid entityId);
}
