using Soundmates.Domain.Entities;

namespace Soundmates.Infrastructure.Identity;

public static class UserMappings
{
    public static User ToDomain(this ApplicationUser user)
    {
        return new User
        {
            Id = user.Id,
            Email = user.Email ?? throw new InvalidOperationException("Email is required."),
            Name = user.Name,
            Description = user.Description,
            BirthYear = user.BirthYear,
            City = user.City,
            Country = user.Country,
            IsActive = user.IsActive
        };
    }
}
