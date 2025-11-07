namespace Soundmates.Domain.Entities;

public class Artist
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required DateOnly BirthDate { get; set; }

    public Guid GenderId { get; set; }
    public Gender Gender { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
