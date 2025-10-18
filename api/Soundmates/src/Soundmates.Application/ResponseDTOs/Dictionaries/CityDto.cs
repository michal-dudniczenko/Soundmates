namespace Soundmates.Application.ResponseDTOs.Dictionaries;

public class CityDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public required Guid CountryId { get; set; }
}
