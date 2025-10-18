namespace Soundmates.Domain.Entities;

public class UserMatchTagPreference
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
