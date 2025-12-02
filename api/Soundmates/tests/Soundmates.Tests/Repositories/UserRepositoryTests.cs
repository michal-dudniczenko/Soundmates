namespace Soundmates.Tests.Repositories;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;

public class UserRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public UserRepositoryTests()
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
    private UserRepository CreateRepo(ApplicationDbContext ctx) => new UserRepository(ctx);

    private User CreateUser(Guid id, string? email = null)
        => new User
        {
            Id = id,
            Email = email ?? "x",
            PasswordHash = "x",
            IsActive = true,
            IsEmailConfirmed = true,
            IsFirstLogin = false
        };

    // ---------------------------------------------------------------------
    // AddAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task AddAsync_AddsUser()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());

        await repo.AddAsync(user);

        Assert.Single(ctx.Users);
    }

    // ---------------------------------------------------------------------
    // CheckIfEmailExistsAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task CheckIfEmailExistsAsync_ReturnsTrue_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid(), "test@test.com");
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var exists = await repo.CheckIfEmailExistsAsync("test@test.com");

        Assert.True(exists);
    }

    [Fact]
    public async Task CheckIfEmailExistsAsync_ReturnsFalse_WhenNotExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var exists = await repo.CheckIfEmailExistsAsync("notfound@test.com");

        Assert.False(exists);
    }

    // ---------------------------------------------------------------------
    // CheckIfExistsActiveAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task CheckIfExistsActiveAsync_ReturnsTrue_WhenActive()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var result = await repo.CheckIfExistsActiveAsync(user.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfExistsActiveAsync_ReturnsFalse_WhenInactive()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        user.IsActive = false;
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var result = await repo.CheckIfExistsActiveAsync(user.Id);

        Assert.False(result);
    }

    // ---------------------------------------------------------------------
    // CheckRefreshTokenGetUserIdAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task CheckRefreshTokenGetUserIdAsync_ReturnsUserId_WhenValid()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = "hash",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        ctx.RefreshTokens.Add(token);
        await ctx.SaveChangesAsync();

        var userId = await repo.CheckRefreshTokenGetUserIdAsync("hash");

        Assert.Equal(user.Id, userId);
    }

    [Fact]
    public async Task CheckRefreshTokenGetUserIdAsync_ReturnsNull_WhenExpired()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };


        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            User = user,
            RefreshTokenHash = "hash",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(-5)
        };
        ctx.RefreshTokens.Add(token);
        await ctx.SaveChangesAsync();

        var dummy = await repo.CheckRefreshTokenGetUserIdAsync("hash");

        Assert.Null(dummy);
    }

    // ---------------------------------------------------------------------
    // DeactivateUserAccountAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task DeactivateUserAccountAsync_UpdatesUserAndRemovesRefreshToken()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = "hash",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        ctx.RefreshTokens.Add(token);

        await ctx.SaveChangesAsync();

        await repo.DeactivateUserAccountAsync(user.Id);

        var updatedUser = await ctx.Users.FindAsync(user.Id);
        Assert.False(updatedUser!.IsActive);
        Assert.True(updatedUser.IsLoggedOut);
        Assert.Empty(ctx.RefreshTokens);
    }

    [Fact]
    public async Task DeactivateUserAccountAsync_Throws_WhenUserNotFound()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => repo.DeactivateUserAccountAsync(Guid.NewGuid()));
    }

    // ---------------------------------------------------------------------
    // GetByEmailAsync / GetByIdAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid(), "email@test.com");
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var result = await repo.GetByEmailAsync("email@test.com");

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var result = await repo.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
    }

    // ---------------------------------------------------------------------
    // LogInUserAsync / LogOutUserAsync / UpdateUserPasswordAsync
    // ---------------------------------------------------------------------
    [Fact]
    public async Task LogInUserAsync_AddsOrUpdatesRefreshToken()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        await repo.LogInUserAsync(user.Id, "hash", DateTime.UtcNow.AddMinutes(10));

        var token = await ctx.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == user.Id);
        Assert.NotNull(token);
        Assert.False(user.IsLoggedOut);
    }

    [Fact]
    public async Task LogOutUserAsync_SetsLoggedOutAndRemovesToken()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = "hash",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };
        ctx.RefreshTokens.Add(token);
        await ctx.SaveChangesAsync();

        await repo.LogOutUserAsync(user.Id);

        Assert.True(user.IsLoggedOut);
        Assert.Empty(ctx.RefreshTokens);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_UpdatesPassword()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        await repo.UpdateUserPasswordAsync(user.Id, "newhash");

        var updatedUser = await ctx.Users.FindAsync(user.Id);
        Assert.Equal("newhash", updatedUser!.PasswordHash);
    }
}
