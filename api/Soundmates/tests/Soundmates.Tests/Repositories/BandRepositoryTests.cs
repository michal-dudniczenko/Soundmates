namespace Soundmates.Tests.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class BandRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public BandRepositoryTests()
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
    private BandRepository CreateRepo(ApplicationDbContext ctx) => new BandRepository(ctx);

    private (User user, Country country, City city) CreateUserWithLocation(Guid? id = null)
    {
        var country = new Country { Id = Guid.NewGuid(), Name = "c" };
        var city = new City { Id = Guid.NewGuid(), Name = "x", Latitude = 0, Longitude = 0, CountryId = country.Id };

        var user = new User
        {
            Id = id ?? Guid.NewGuid(),
            Email = "a",
            PasswordHash = "b",
            CountryId = country.Id,
            CityId = city.Id,
            Country = country,
            City = city,
            IsActive = true,
            IsEmailConfirmed = true,
            IsFirstLogin = false
        };

        return (user, country, city);
    }

    // ---------------------------------------------------------
    // GetByUserIdAsync
    // ---------------------------------------------------------
    [Fact]
    public async Task GetByUserIdAsync_ReturnsBand_WhenExists()
    {
        using var ctx = CreateDb();

        var (user, country, city) = CreateUserWithLocation();
        ctx.Countries.Add(country);
        ctx.Cities.Add(city);
        ctx.Users.Add(user);

        var band = new Band { UserId = user.Id, User = user };
        ctx.Bands.Add(band);

        await ctx.SaveChangesAsync();

        var repo = CreateRepo(ctx);
        var result = await repo.GetByUserIdAsync(user.Id);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNull_WhenNotExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.GetByUserIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ---------------------------------------------------------
    // GetPotentialMatchesAsync
    // ---------------------------------------------------------
    [Fact]
    public async Task GetPotentialMatchesAsync_Throws_WhenPreferencesMissing()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.GetPotentialMatchesAsync(Guid.NewGuid(), 10, 0));
    }

    [Fact]
    public async Task GetPotentialMatchesAsync_ReturnsEmpty_WhenShowBandsFalse()
    {
        using var ctx = CreateDb();

        var (user, country, city) = CreateUserWithLocation();

        ctx.Countries.Add(country);
        ctx.Cities.Add(city);
        ctx.Users.Add(user);

        ctx.UserMatchPreferences.Add(new UserMatchPreference
        {
            UserId = user.Id,
            ShowBands = false,
            User = user
        });

        await ctx.SaveChangesAsync();

        var repo = CreateRepo(ctx);

        var result = await repo.GetPotentialMatchesAsync(user.Id, 10, 0);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPotentialMatchesAsync_ReturnsOneBand_WhenSimpleMatch()
    {
        using var ctx = CreateDb();

        var (mainUser, c1, c2) = CreateUserWithLocation();
        ctx.Users.Add(mainUser);
        ctx.Countries.Add(c1);
        ctx.Cities.Add(c2);

        ctx.UserMatchPreferences.Add(new UserMatchPreference
        {
            UserId = mainUser.Id,
            ShowBands = true,
            User = mainUser,
        });

        // Candidate band
        var (u2, c3, c4) = CreateUserWithLocation();
        ctx.Countries.Add(c3);
        ctx.Cities.Add(c4);
        ctx.Users.Add(u2);

        var band = new Band { UserId = u2.Id, User = u2 };
        ctx.Bands.Add(band);

        await ctx.SaveChangesAsync();

        var repo = CreateRepo(ctx);

        var result = await repo.GetPotentialMatchesAsync(mainUser.Id, 10, 0);

        Assert.Single(result);
    }

    // ---------------------------------------------------------
    // UpdateAddAsync
    // ---------------------------------------------------------
    [Fact]
    public async Task UpdateAddAsync_Throws_WhenUserNotFound()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var band = new Band { UserId = Guid.NewGuid(), User = new User { Id = Guid.NewGuid(), Email = "x", PasswordHash = "y" } };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.UpdateAddAsync(band, [], [], []));
    }

    [Fact]
    public async Task UpdateAddAsync_InsertsNewBand_WhenBandDoesNotExist()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var (user, country, city) = CreateUserWithLocation();

        ctx.Countries.Add(country);
        ctx.Cities.Add(city);
        ctx.Users.Add(user);

        var tagCat = new TagCategory { Id = Guid.NewGuid(), Name = "band", IsForBand = true };
        var tag = new Tag { Id = Guid.NewGuid(), Name = "t", TagCategoryId = tagCat.Id, TagCategory = tagCat };
        ctx.TagCategories.Add(tagCat);
        ctx.Tags.Add(tag);

        var sample = new MusicSample { Id = Guid.NewGuid(), FileName = "x", UserId = user.Id, DisplayOrder = 0 };
        var pic = new ProfilePicture { Id = Guid.NewGuid(), FileName = "y", UserId = user.Id, DisplayOrder = 0 };

        user.MusicSamples.Add(sample);
        user.ProfilePictures.Add(pic);

        await ctx.SaveChangesAsync();

        var band = new Band
        {
            UserId = user.Id,
            User = user,
        };

        await repo.UpdateAddAsync(
            band,
            new List<Guid> { tag.Id },
            new List<Guid> { sample.Id },
            new List<Guid> { pic.Id }
        );

        Assert.Equal(1, ctx.Bands.Count());
    }

    [Fact]
    public async Task UpdateAddAsync_UpdatesExistingBand_WithNewMembers()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var (user, country, city) = CreateUserWithLocation();

        ctx.Countries.Add(country);
        ctx.Cities.Add(city);
        ctx.Users.Add(user);

        var band = new Band { UserId = user.Id, User = user };
        ctx.Bands.Add(band);

        var role = new BandRole { Id = Guid.NewGuid(), Name = "Role" };
        ctx.BandRoles.Add(role);

        await ctx.SaveChangesAsync();

        var updated = new Band
        {
            UserId = user.Id,
            User = user,
            Members =
            {
                new BandMember { Name = "A", Age = 20, DisplayOrder = 0, BandRoleId = role.Id }
            }
        };

        await repo.UpdateAddAsync(updated, [], [], []);

        Assert.Single(ctx.BandMembers);
    }

    [Fact]
    public async Task UpdateAddAsync_Throws_WhenDuplicateSampleIds()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var (user, country, city) = CreateUserWithLocation();

        ctx.Countries.Add(country);
        ctx.Cities.Add(city);
        ctx.Users.Add(user);

        await ctx.SaveChangesAsync();

        var band = new Band { UserId = user.Id, User = user };

        var dup = Guid.NewGuid();
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.UpdateAddAsync(band, [], new List<Guid> { dup, dup }, []));
    }

    [Fact]
    public async Task UpdateAddAsync_Throws_WhenDuplicatePictureIds()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var (user, country, city) = CreateUserWithLocation();

        ctx.Countries.Add(country);
        ctx.Cities.Add(city);
        ctx.Users.Add(user);

        await ctx.SaveChangesAsync();

        var band = new Band { UserId = user.Id, User = user };

        var dup = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.UpdateAddAsync(band, [], [], new List<Guid> { dup, dup }));
    }
}
