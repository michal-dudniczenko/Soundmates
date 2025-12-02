namespace Soundmates.Tests.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

public class MusicSampleRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public MusicSampleRepositoryTests()
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
    private MusicSampleRepository CreateRepo(ApplicationDbContext ctx) => new MusicSampleRepository(ctx);

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
    public async Task AddAsync_AddsSample()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var sample = new MusicSample
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FileName = "test.mp3",
            DisplayOrder = 0
        };

        await repo.AddAsync(sample);

        Assert.Single(ctx.MusicSamples);
    }

    // ---------------------------------------------------------------------
    // GetByIdAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task GetByIdAsync_ReturnsSample_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var sample = new MusicSample
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            FileName = "file.mp3",
            DisplayOrder = 0
        };
        ctx.MusicSamples.Add(sample);

        await ctx.SaveChangesAsync();

        var result = await repo.GetByIdAsync(sample.Id);

        Assert.NotNull(result);
        Assert.Equal(sample.Id, result!.Id);
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
    // GetUserMusicSamplesCountAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task GetUserMusicSamplesCountAsync_ReturnsCorrectCount()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user1 = CreateUser(Guid.NewGuid());
        var user2 = CreateUser(Guid.NewGuid());
        ctx.Users.AddRange(user1, user2);

        ctx.MusicSamples.AddRange(
            new MusicSample { Id = Guid.NewGuid(), UserId = user1.Id, FileName = "a", DisplayOrder = 0 },
            new MusicSample { Id = Guid.NewGuid(), UserId = user1.Id, FileName = "b", DisplayOrder = 1 },
            new MusicSample { Id = Guid.NewGuid(), UserId = user2.Id, FileName = "c", DisplayOrder = 0 }
        );

        await ctx.SaveChangesAsync();

        var count = await repo.GetUserMusicSamplesCountAsync(user1.Id);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetUserMusicSamplesCountAsync_ReturnsZero_WhenNoneExist()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var count = await repo.GetUserMusicSamplesCountAsync(user.Id);

        Assert.Equal(0, count);
    }

    // ---------------------------------------------------------------------
    // RemoveAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task RemoveAsync_RemovesSample_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var sample = new MusicSample
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            DisplayOrder = 0,
            FileName = "x"
        };
        ctx.MusicSamples.Add(sample);

        await ctx.SaveChangesAsync();

        await repo.RemoveAsync(sample.Id);

        Assert.Empty(ctx.MusicSamples);
    }

    [Fact]
    public async Task RemoveAsync_Throws_WhenSampleNotFound()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.RemoveAsync(Guid.NewGuid()));
    }
}
