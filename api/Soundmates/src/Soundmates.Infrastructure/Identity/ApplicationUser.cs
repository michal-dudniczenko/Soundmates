using Microsoft.AspNetCore.Identity;

namespace Soundmates.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int BirthYear { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public bool IsActive { get; set; } = true;
}
