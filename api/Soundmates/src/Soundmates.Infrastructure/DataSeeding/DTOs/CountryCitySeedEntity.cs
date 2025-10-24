namespace Soundmates.Infrastructure.DataSeeding.DTOs;

public class CountryCitySeedEntity
{
    public required string City { get; set; }
    public required string Country { get; set; }
    public required double Lat { get; set; }
    public required double Lng { get; set; }
}
