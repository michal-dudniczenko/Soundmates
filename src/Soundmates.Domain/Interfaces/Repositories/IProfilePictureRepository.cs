using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IProfilePictureRepository
{
    Task<ProfilePicture?> GetByIdAsync(Guid entityId);
    Task AddAsync(ProfilePicture entity);
    Task RemoveAsync(Guid entityId);
    Task<int> GetUserProfilePicturesCountAsync(Guid userId);
}
