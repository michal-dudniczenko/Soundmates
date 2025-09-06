using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public abstract class UserMedia
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [MaxLength(100)]
    public required string FileName { get; set; }
    public required Guid UserId { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
