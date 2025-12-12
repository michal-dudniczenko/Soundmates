namespace Soundmates.Tests.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class ArtistRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _dbOptions;

    public ArtistRepositoryTests()
    {
        // create shared sqlite connection
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        // ensure schema is created
        using var ctx = new ApplicationDbContext(_dbOptions);
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private ApplicationDbContext CreateDb() => new ApplicationDbContext(_dbOptions);

    private ArtistRepository CreateRepo(ApplicationDbContext ctx) => new ArtistRepository(ctx);

    // --------------------------------------------------------------------
    //  GetByUserIdAsync
    // --------------------------------------------------------------------
    [Fact]
    public async Task GetByUserIdAsync_ReturnsArtist_WhenExists()
    {
        using var ctx = CreateDb();

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };
        var artist = new Artist { UserId = userId, User = user, BirthDate = DateOnly.MinValue, Gender = gender };

        ctx.Artists.Add(artist);
        await ctx.SaveChangesAsync();

        var repo = CreateRepo(ctx);

        var result = await repo.GetByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNull_WhenNotExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.GetByUserIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // --------------------------------------------------------------------
    //  GetPotentialMatchesAsync
    // --------------------------------------------------------------------
    [Fact]
    public async Task GetPotentialMatchesAsync_Throws_WhenPreferencesMissing()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.GetPotentialMatchesAsync(Guid.NewGuid(), 10, 0));
    }

    [Fact]
    public async Task GetPotentialMatchesAsync_ReturnsEmpty_WhenShowArtistsFalse()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            CityId = null,
            Email = string.Empty,
            PasswordHash = string.Empty
        };

        ctx.Users.Add(user);

        ctx.UserMatchPreferences.Add(new UserMatchPreference
        {
            UserId = userId,
            ShowArtists = false,
            User = user
        });

        await ctx.SaveChangesAsync();

        var result = await repo.GetPotentialMatchesAsync(userId, 10, 0);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPotentialMatchesAsync_FiltersOutLikedUsers()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var mainUser = new User
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            IsEmailConfirmed = true,
            IsFirstLogin = false,
            Email = string.Empty,
            PasswordHash = string.Empty,
            Country = country,
            City = city
        };

        var matchUser = new User
        {
            Id = Guid.NewGuid(),
            IsActive = true,
            IsEmailConfirmed = true,
            IsFirstLogin = false,
            Email = string.Empty,
            PasswordHash = string.Empty,
            Country = country,
            City = city
        };

        ctx.Users.AddRange(mainUser, matchUser);

        ctx.UserMatchPreferences.Add(new UserMatchPreference
        {
            ShowArtists = true,
            User = mainUser,            
        });

        ctx.Artists.Add(new Artist
        {
            User = matchUser,
            BirthDate = DateOnly.MinValue,
            Gender = gender
        });

        ctx.Likes.Add(new Like
        {
            GiverId = mainUser.Id,
            ReceiverId = matchUser.Id
        });

        await ctx.SaveChangesAsync();

        var result = await repo.GetPotentialMatchesAsync(mainUser.Id, 10, 0);

        Assert.Empty(result);
    }


    // --------------------------------------------------------------------
    //  UpdateAddAsync
    // --------------------------------------------------------------------
    [Fact]
    public async Task UpdateAddAsync_Throws_WhenUserNotFound()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var artist = new Artist
        {
            UserId = Guid.NewGuid(),
            User = new User
            {
                Email = string.Empty,
                PasswordHash = string.Empty
            },
            BirthDate = DateOnly.MinValue
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.UpdateAddAsync(artist, [], [], []));
    }

    [Fact]
    public async Task UpdateAddAsync_UpdatesArtist_WhenArtistExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };
        var artist = new Artist { UserId = userId, User = user, BirthDate = DateOnly.MinValue, Gender = gender };

        ctx.Artists.Add(artist);
        await ctx.SaveChangesAsync();

        // Tag
        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            TagCategory = new TagCategory { Name = string.Empty, IsForBand = false }
        };
        ctx.Tags.Add(tag);

        // Music sample
        var sample = new MusicSample
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayOrder = 0,
            FileName = string.Empty
        };
        ctx.MusicSamples.Add(sample);

        // Profile picture
        var picture = new ProfilePicture
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayOrder = 0,
            FileName = string.Empty
        };
        ctx.ProfilePictures.Add(picture);

        await ctx.SaveChangesAsync();

        var updated = new Artist
        {
            Id = artist.Id,
            User = new User
            {
                Id = userId,
                Name = "New Name",
                Description = "New Desc",
                CountryId = country.Id,
                CityId = city.Id,
                Email = string.Empty,
                PasswordHash = string.Empty
            },
            UserId = userId,
            BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
            GenderId = gender.Id
        };

        await repo.UpdateAddAsync(
            updated,
            new List<Guid> { tag.Id },
            new List<Guid> { sample.Id },
            new List<Guid> { picture.Id }
        );

        var saved = await ctx.Artists.Include(a => a.User).FirstAsync();

        Assert.Equal("New Name", saved.User.Name);
        Assert.Equal(gender.Id, saved.GenderId);
        Assert.Single(saved.User.Tags);
    }

    [Fact]
    public async Task UpdateAddAsync_Throws_WhenDuplicateMusicSampleIds()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var userId = Guid.NewGuid();
        var user = new User { Id = userId,
            Email = string.Empty,
            PasswordHash = string.Empty
        };
        ctx.Users.Add(user);

        ctx.Tags.Add(new Tag
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            TagCategory = new TagCategory { IsForBand = false, Name = string.Empty }
        });

        ctx.MusicSamples.Add(new MusicSample
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayOrder = 0,
            FileName = string.Empty
        });

        await ctx.SaveChangesAsync();

        var artist = new Artist { UserId = userId, User = user, BirthDate = DateOnly.MinValue };

        var duplicateList = new[]
        {
            Guid.NewGuid(),
            Guid.NewGuid() // duplicates
        };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            repo.UpdateAddAsync(artist, [], duplicateList, []));
    }
}

