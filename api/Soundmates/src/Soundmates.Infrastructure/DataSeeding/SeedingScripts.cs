using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.DataSeeding.DTOs;
using System.Text.Json;

namespace Soundmates.Infrastructure.DataSeeding;

public static class SeedingScripts
{
    private static readonly JsonSerializerOptions serializerOptions = new ()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string SeedDataDirectoryPath = "DataSeeding/SeedData";

    private const string CountriesCitiesDataFileName = "countries-cities.json";
    private const string GendersDataFileName = "genders.json";
    private const string BandRolesDataFileName = "band-roles.json";
    private const string ArtistTagCategoriesDataFileName = "artist-tag-categories.json";
    private const string BandTagCategoriesDataFileName = "band-tag-categories.json";
    private const string ArtistTagsDataFileName = "artist-tags.json";
    private const string BandTagsDataFileName = "band-tags.json";

    public static async Task SeedData(DbContext context, CancellationToken ct)
    {
        await SeedCountriesCities(context, ct);
        await SeedGenders(context, ct);
        await SeedBandRoles(context, ct);
        await SeedArtistTagCategories(context, ct);
        await SeedBandTagCategories(context, ct);
        await SeedArtistTags(context, ct);
        await SeedBandTags(context, ct);
    }

    private static async Task SeedCountriesCities(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<CountryCitySeedEntity>(CountriesCitiesDataFileName, ct);

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

        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedGenders(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<GenderSeedEntity>(GendersDataFileName, ct);

        var genders = data.Select(x => x.Name).Distinct().Select(g => new Gender
        {
            Name = g
        }).ToList();

        context.Set<Gender>().AddRange(genders);

        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedBandRoles(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<BandRoleSeedEntity>(BandRolesDataFileName, ct);

        var bandRoles = data.Select(x => x.Name).Distinct().Select(br => new BandRole
        {
            Name = br
        }).ToList();

        context.Set<BandRole>().AddRange(bandRoles);

        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedArtistTagCategories(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<TagCategorySeedEntity>(ArtistTagCategoriesDataFileName, ct);

        var tagCategories = data.Select(tc => new TagCategory
        {
            Name = tc.Name,
            IsForBand = false
        }).ToList();

        context.Set<TagCategory>().AddRange(tagCategories);

        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedBandTagCategories(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<TagCategorySeedEntity>(BandTagCategoriesDataFileName, ct);

        var tagCategories = data.Select(tc => new TagCategory
        {
            Name = tc.Name,
            IsForBand = true
        }).ToList();

        context.Set<TagCategory>().AddRange(tagCategories);

        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedArtistTags(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<TagSeedEntity>(ArtistTagsDataFileName, ct);

        var tagCategories = await context.Set<TagCategory>()
            .Where(tc => !tc.IsForBand)
            .ToListAsync(ct);

        List<Tag> tags = [];

        foreach (var entry in data)
        {
            var category = tagCategories.FirstOrDefault(c => c.Name == entry.CategoryName);

            if (category is null) continue;

            tags.Add(new Tag
            {
                Name = entry.Name,
                TagCategoryId = category.Id
            });
        }

        context.Set<Tag>().AddRange(tags);

        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedBandTags(DbContext context, CancellationToken ct)
    {
        var data = await DeserializeSeedCollection<TagSeedEntity>(BandTagsDataFileName, ct);

        var tagCategories = await context.Set<TagCategory>()
            .Where(tc => tc.IsForBand)
            .ToListAsync(ct);

        List<Tag> tags = [];

        foreach (var entry in data)
        {
            var category = tagCategories.FirstOrDefault(c => c.Name == entry.CategoryName);

            if (category is null) continue;

            tags.Add(new Tag
            {
                Name = entry.Name,
                TagCategoryId = category.Id
            });
        }

        context.Set<Tag>().AddRange(tags);

        await context.SaveChangesAsync(ct);
    }

    private static async Task<List<T>> DeserializeSeedCollection<T>(string seedFileName, CancellationToken ct)
    {
        var path = Path.Combine(AppContext.BaseDirectory, SeedDataDirectoryPath, seedFileName);

        var jsonString = await File.ReadAllTextAsync(path, ct);

        var data = JsonSerializer.Deserialize<List<T>>(jsonString, serializerOptions)
            ?? throw new InvalidOperationException("Failed to deserialize json data.");

        return data;
    }
}
