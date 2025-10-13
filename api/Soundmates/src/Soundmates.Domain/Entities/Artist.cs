namespace Soundmates.Domain.Entities;

public class Artist : UserBase
{
    public DateOnly? BirthDate { get; set; } = null;
}
