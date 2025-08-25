using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public abstract class UserMedia
{
    public int Id { get; set; }

    [MaxLength(100)]
    public required string FileName { get; set; }
    public required int UserId { get; set; }
}
