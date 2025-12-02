namespace Soundmates.Tests.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

public class ProfilePictureRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public ProfilePictureRepositoryTests()
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
    private ProfilePictureRepository CreateRepo(ApplicationDbContext ctx) => new ProfilePictureRepository(ctx);

    private User CreateUser(Guid id) => new User
    {
        Id = id,
        Email = "x",
        PasswordHash = "x",
        IsActive = true,
        IsEmailConfirmed = true
    };

    // ---------------------------------------------------------------------
    // AddAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task AddAsync_AddsPicture()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var picture = new ProfilePicture
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FileName = "pic.jpg",
            DisplayOrder = 0
        };

        await repo.AddAsync(picture);

        Assert.Single(ctx.ProfilePictures);
    }

    // ---------------------------------------------------------------------
    // GetByIdAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task GetByIdAsync_ReturnsPicture_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var picture = new ProfilePicture
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FileName = "pic.jpg",
            DisplayOrder = 0
        };
        ctx.ProfilePictures.Add(picture);
        await ctx.SaveChangesAsync();

        var result = await repo.GetByIdAsync(picture.Id);

        Assert.NotNull(result);
        Assert.Equal(picture.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    // ---------------------------------------------------------------------
    // GetUserProfilePicturesCountAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task GetUserProfilePicturesCountAsync_ReturnsCorrectCount()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user1 = CreateUser(Guid.NewGuid());
        var user2 = CreateUser(Guid.NewGuid());
        ctx.Users.AddRange(user1, user2);

        ctx.ProfilePictures.AddRange(
            new ProfilePicture { Id = Guid.NewGuid(), UserId = user1.Id, FileName = "a", DisplayOrder = 0 },
            new ProfilePicture { Id = Guid.NewGuid(), UserId = user1.Id, FileName = "b", DisplayOrder = 1 },
            new ProfilePicture { Id = Guid.NewGuid(), UserId = user2.Id, FileName = "c", DisplayOrder = 0 }
        );

        await ctx.SaveChangesAsync();

        var count = await repo.GetUserProfilePicturesCountAsync(user1.Id);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetUserProfilePicturesCountAsync_ReturnsZero_WhenNoneExist()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var count = await repo.GetUserProfilePicturesCountAsync(user.Id);

        Assert.Equal(0, count);
    }

    // ---------------------------------------------------------------------
    // RemoveAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task RemoveAsync_RemovesPicture_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var picture = new ProfilePicture
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FileName = "pic.jpg",
            DisplayOrder = 0
        };
        ctx.ProfilePictures.Add(picture);

        await ctx.SaveChangesAsync();

        await repo.RemoveAsync(picture.Id);

        Assert.Empty(ctx.ProfilePictures);
    }

    [Fact]
    public async Task RemoveAsync_Throws_WhenPictureNotFound()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.RemoveAsync(Guid.NewGuid()));
    }
}
