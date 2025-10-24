using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.DataSeeding.DTOs;
using System.Text.Json;

namespace Soundmates.Infrastructure.DataSeeding;

public static class SeedingScripts
{
    private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private const string SeedDataDirectoryName = "DataSeeding";
    private const string CountriesCitiesDataFileName = "countries-cities.json";

    public async static Task SeedCountriesCities(DbContext context, CancellationToken ct)
    {
        var path = Path.Combine(AppContext.BaseDirectory, SeedDataDirectoryName, CountriesCitiesDataFileName);

        var jsonString = await File.ReadAllTextAsync(path, ct);

        var data = JsonSerializer.Deserialize<List<CountryCitySeedEntity>>(jsonString, serializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize json data.");

        var countries = data.Select(x => x.Country).Distinct().Select(c => new Country
        {
            Name = c
        }).ToList();

        List<City> cities = [];

        foreach (var entry in data)
        {
            var country = countries.FirstOrDefault(c => c.Name == entry.Country);

            if (country is null) continue;

            cities.Add(new City
            {
                Name = entry.City,
                Latitude = entry.Lat,
                Longitude = entry.Lng,
                CountryId = country.Id
            });
        }

        context.Set<Country>().AddRange(countries);
        context.Set<City>().AddRange(cities);
    }
}
