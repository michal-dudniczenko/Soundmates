namespace Soundmates.Tests.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class DictionaryRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public DictionaryRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        using var ctx = new ApplicationDbContext(_options);
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private ApplicationDbContext CreateDb() => new ApplicationDbContext(_options);
    private DictionaryRepository CreateRepo(ApplicationDbContext ctx) => new DictionaryRepository(ctx);

    // ---------------------------------------------------------
    // Band Roles
    // ---------------------------------------------------------
    [Fact]
    public async Task GetAllBandRolesAsync_ReturnsOrderedList()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        ctx.BandRoles.AddRange(
            new BandRole { Id = Guid.NewGuid(), Name = "Drummer" },
            new BandRole { Id = Guid.NewGuid(), Name = "Bassist" }
        );
        await ctx.SaveChangesAsync();

        var result = await repo.GetAllBandRolesAsync();

        Assert.Equal(2, result.Count());
        Assert.Equal("Bassist", result.First().Name);
    }

    // ---------------------------------------------------------
    // Countries
    // ---------------------------------------------------------
    [Fact]
    public async Task GetAllCountriesAsync_ReturnsOrdered()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        ctx.Countries.AddRange(
            new Country { Id = Guid.NewGuid(), Name = "USA" },
            new Country { Id = Guid.NewGuid(), Name = "Canada" }
        );
        await ctx.SaveChangesAsync();

        var result = await repo.GetAllCountriesAsync();

        Assert.Equal(2, result.Count());
        Assert.Equal("Canada", result.First().Name);
    }

    // ---------------------------------------------------------
    // Cities
    // ---------------------------------------------------------
    [Fact]
    public async Task GetAllCountryCitiesAsync_ReturnsCitiesForCountry()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country1 = new Country { Id = Guid.NewGuid(), Name = "A" };
        var country2 = new Country { Id = Guid.NewGuid(), Name = "B" };

        ctx.Countries.AddRange(country1, country2);

        ctx.Cities.AddRange(
            new City { Id = Guid.NewGuid(), Name = "X City", Latitude = 0, Longitude = 0, CountryId = country1.Id },
            new City { Id = Guid.NewGuid(), Name = "A City", Latitude = 0, Longitude = 0, CountryId = country1.Id },
            new City { Id = Guid.NewGuid(), Name = "Z City", Latitude = 0, Longitude = 0, CountryId = country2.Id }
        );

        await ctx.SaveChangesAsync();

        var result = await repo.GetAllCountryCitiesAsync(country1.Id);

        Assert.Equal(2, result.Count());
        Assert.Equal("A City", result.First().Name); // ordered
    }

    // ---------------------------------------------------------
    // Genders
    // ---------------------------------------------------------
    [Fact]
    public async Task GetAllGendersAsync_ReturnsOrdered()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        ctx.Genders.AddRange(
            new Gender { Id = Guid.NewGuid(), Name = "Male" },
            new Gender { Id = Guid.NewGuid(), Name = "Female" }
        );
        await ctx.SaveChangesAsync();

        var result = await repo.GetAllGendersAsync();

        Assert.Equal(2, result.Count());
        Assert.Equal("Female", result.First().Name);
    }

    // ---------------------------------------------------------
    // Tag Categories
    // ---------------------------------------------------------
    [Fact]
    public async Task GetAllTagCategoriesAsync_ReturnsOrdered()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        ctx.TagCategories.AddRange(
            new TagCategory { Id = Guid.NewGuid(), Name = "Zzz", IsForBand = false },
            new TagCategory { Id = Guid.NewGuid(), Name = "Aaa", IsForBand = true }
        );

        await ctx.SaveChangesAsync();

        var result = await repo.GetAllTagCategoriesAsync();

        Assert.Equal(2, result.Count());
        Assert.Equal("Aaa", result.First().Name);
    }

    // ---------------------------------------------------------
    // Tags
    // ---------------------------------------------------------
    [Fact]
    public async Task GetAllTagsAsync_ReturnsOrdered()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var cat = new TagCategory { Id = Guid.NewGuid(), Name = "Cat", IsForBand = false };
        ctx.TagCategories.Add(cat);

        ctx.Tags.AddRange(
            new Tag { Id = Guid.NewGuid(), Name = "Z2", TagCategoryId = cat.Id },
            new Tag { Id = Guid.NewGuid(), Name = "A1", TagCategoryId = cat.Id }
        );

        await ctx.SaveChangesAsync();

        var result = await repo.GetAllTagsAsync();

        Assert.Equal(2, result.Count());
        Assert.Equal("A1", result.First().Name);
    }
}

