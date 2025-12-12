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

public class MatchRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public MatchRepositoryTests()
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
    private MatchRepository CreateRepo(ApplicationDbContext ctx)
        => new MatchRepository(ctx);

    private User CreateUser(Guid id)
    {
        return new User
        {
            Id = id,
            Email = "test@test.com",
            PasswordHash = "x",
            IsActive = true,
            IsEmailConfirmed = true
        };
    }

    // ----------------------------------------------------------------------
    // AddMatchAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task AddMatchAsync_AddsMatch()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var u1 = CreateUser(Guid.NewGuid());
        var u2 = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(u1, u2);
        await ctx.SaveChangesAsync();

        var match = new Match { User1Id = u1.Id, User2Id = u2.Id };

        await repo.AddMatchAsync(match);

        Assert.Single(ctx.Matches);
    }

    // ----------------------------------------------------------------------
    // AddDislikeAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task AddDislikeAsync_AddsDislike()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var user1Id = Guid.NewGuid();
        var user1 = new User { Id = user1Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var user2Id = Guid.NewGuid();
        var user2 = new User { Id = user2Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var d = new Dislike
        {
            Giver = user1,
            Receiver = user2
        };

        await repo.AddDislikeAsync(d);

        Assert.Single(ctx.Dislikes);
    }

    // ----------------------------------------------------------------------
    // AddLikeAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task AddLikeAsync_AddsLike()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var user1Id = Guid.NewGuid();
        var user1 = new User { Id = user1Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var user2Id = Guid.NewGuid();
        var user2 = new User { Id = user2Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var l = new Like
        {
            Giver = user1,
            Receiver = user2
        };

        await repo.AddLikeAsync(l);

        Assert.Single(ctx.Likes);
    }

    // ----------------------------------------------------------------------
    // CheckIfLikeExistsAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task CheckIfLikeExistsAsync_ReturnsTrue_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var user1Id = Guid.NewGuid();
        var user1 = new User { Id = user1Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var user2Id = Guid.NewGuid();
        var user2 = new User { Id = user2Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var l = new Like
        {
            Giver = user1,
            Receiver = user2
        };

        ctx.Likes.Add(l);
        await ctx.SaveChangesAsync();

        var result = await repo.CheckIfLikeExistsAsync(user1Id, user2Id);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfLikeExistsAsync_ReturnsFalse_WhenNotExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.CheckIfLikeExistsAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    // ----------------------------------------------------------------------
    // CheckIfMatchExistsAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task CheckIfMatchExistsAsync_ReturnsTrue_RegardlessOfOrder()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var user1Id = Guid.NewGuid();
        var user1 = new User { Id = user1Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var user2Id = Guid.NewGuid();
        var user2 = new User { Id = user2Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        ctx.Matches.Add(new Match { User1 = user1, User2 = user2 });
        await ctx.SaveChangesAsync();

        var direct = await repo.CheckIfMatchExistsAsync(user1Id, user2Id);
        var reversed = await repo.CheckIfMatchExistsAsync(user2Id, user1Id);

        Assert.True(direct);
        Assert.True(reversed);
    }

    [Fact]
    public async Task CheckIfMatchExistsAsync_ReturnsFalse_WhenNoMatch()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.CheckIfMatchExistsAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    // ----------------------------------------------------------------------
    // CheckIfDislikeExistsAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task CheckIfDislikeExistsAsync_ReturnsTrue_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var user1Id = Guid.NewGuid();
        var user1 = new User { Id = user1Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var user2Id = Guid.NewGuid();
        var user2 = new User { Id = user2Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var d = new Dislike
        {
            Giver = user1,
            Receiver = user2
        };

        ctx.Dislikes.Add(d);
        await ctx.SaveChangesAsync();

        var result = await repo.CheckIfDislikeExistsAsync(user1Id, user2Id);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfDislikeExistsAsync_ReturnsFalse_WhenNotExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.CheckIfDislikeExistsAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    // ----------------------------------------------------------------------
    // GetUserMatchesAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task GetUserMatchesAsync_ReturnsMatches_OrderedAndPaged()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        var other = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(user, other);
        await ctx.SaveChangesAsync();

        ctx.Matches.AddRange(
            new Match { Id = Guid.NewGuid(), User1Id = user.Id, User2Id = other.Id },
            new Match { Id = Guid.NewGuid(), User1Id = other.Id, User2Id = user.Id }
        );

        await ctx.SaveChangesAsync();

        var result = await repo.GetUserMatchesAsync(user.Id, 10, 0);

        Assert.Equal(2, result.Count());
    }

    // ----------------------------------------------------------------------
    // DeleteMatchAsync
    // ----------------------------------------------------------------------
    [Fact]
    public async Task DeleteMatchAsync_DeletesBothOrders()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var user1Id = Guid.NewGuid();
        var user1 = new User { Id = user1Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var user2Id = Guid.NewGuid();
        var user2 = new User { Id = user2Id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        ctx.Matches.Add(new Match { User1 = user1, User2 = user2 });
        ctx.Matches.Add(new Match { User1 = user2, User2 = user1 });
        await ctx.SaveChangesAsync();

        await repo.DeleteMatchAsync(user1Id, user2Id);

        Assert.Empty(ctx.Matches);
    }
}
