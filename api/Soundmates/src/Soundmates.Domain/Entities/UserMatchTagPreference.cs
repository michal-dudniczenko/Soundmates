namespace Soundmates.Domain.Entities;

public class UserMatchTagPreference
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    public Guid UserMatchPreferenceId { get; set; }
    public UserMatchPreference UserMatchPreference { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
