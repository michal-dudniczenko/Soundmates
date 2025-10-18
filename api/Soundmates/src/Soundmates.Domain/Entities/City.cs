namespace Soundmates.Domain.Entities;

public class City
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Guid CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
