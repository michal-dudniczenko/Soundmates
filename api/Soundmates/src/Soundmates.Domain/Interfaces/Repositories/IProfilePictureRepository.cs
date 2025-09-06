using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IProfilePictureRepository : IBaseRepository<ProfilePicture>
{
    Task<IEnumerable<ProfilePicture>> GetUserProfilePicturesAsync(int userId, int limit, int offset);
    Task<int> GetUserProfilePicturesCountAsync(int userId);
    Task<bool> MoveDisplayOrderUpAsync(int pictureId);
    Task<bool> MoveDisplayOrderDownAsync(int pictureId);
}
