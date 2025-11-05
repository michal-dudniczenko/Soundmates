namespace Soundmates.Domain.Entities;

public abstract class UserMedia
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string FileName { get; set; }
    public required int DisplayOrder { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
