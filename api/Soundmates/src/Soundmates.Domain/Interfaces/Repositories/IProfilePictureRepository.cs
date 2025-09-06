using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IProfilePictureRepository : IBaseRepository<ProfilePicture>
{
    Task<IEnumerable<ProfilePicture>> GetUserProfilePicturesAsync(Guid userId, int limit, int offset);
    Task<int> GetUserProfilePicturesCountAsync(Guid userId);
    Task<bool> MoveDisplayOrderUpAsync(Guid pictureId);
    Task<bool> MoveDisplayOrderDownAsync(Guid pictureId);
}
