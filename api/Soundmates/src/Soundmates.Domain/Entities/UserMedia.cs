using System.ComponentModel.DataAnnotations;

namespace Soundmates.Domain.Entities;

public abstract class UserMedia
{
    public int Id { get; set; }

    [MaxLength(100)]
    [RegularExpression(@"\S+", ErrorMessage = "File name cannot be empty or whitespace.")]
    public required string FileName { get; set; }
    public required int UserId { get; set; }
}
