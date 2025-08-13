using Soundmates.Domain.Entities;

namespace Soundmates.Domain.Interfaces.Repositories;

public interface IProfilePictureRepository : IBaseRepository<ProfilePicture>
{
    Task<IEnumerable<ProfilePicture>> GetUserProfilePicturesAsync(int userId, int limit = 50, int offset = 0);
}
