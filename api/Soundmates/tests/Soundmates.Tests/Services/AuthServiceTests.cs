namespace Soundmates.Tests.Services;

using Moq;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Infrastructure.Services.Auth;
using System;
using System.Threading.Tasks;
using Xunit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
    }

    [Fact]
    public void Constructor_Throws_OnEmptySecretKey()
    {
        Assert.Throws<ArgumentException>(() => new AuthService("", _userRepoMock.Object));
    }

    [Fact]
    public void GenerateAccessToken_ReturnsToken()
    {
        var service = new AuthService("supersecretkey1234supersecretkey1234", _userRepoMock.Object);
        var token = service.GenerateAccessToken(Guid.NewGuid());
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsBase64String()
    {
        var service = new AuthService("supersecretkey", _userRepoMock.Object);
        var token = service.GenerateRefreshToken(Guid.NewGuid());
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GetPasswordHash_And_VerifyPasswordHash_Works()
    {
        var service = new AuthService("supersecretkey", _userRepoMock.Object);
        var password = "Password123!";
        var hash = service.GetPasswordHash(password);

        Assert.True(service.VerifyPasswordHash(password, hash));
        Assert.False(service.VerifyPasswordHash("WrongPassword", hash));
    }

    [Fact]
    public void GetRefreshTokenHash_And_VerifyRefreshTokenHash_Works()
    {
        var service = new AuthService("supersecretkey", _userRepoMock.Object);
        var token = "someRandomToken";
        var hash = service.GetRefreshTokenHash(token);

        Assert.True(service.VerifyRefreshTokenHash(token, hash));
        Assert.False(service.VerifyRefreshTokenHash("wrongToken", hash));
    }

    [Fact]
    public async Task GetAuthorizedUserAsync_ReturnsNull_ForInvalidSubClaim()
    {
        var service = new AuthService("supersecretkey", _userRepoMock.Object);
        var user = await service.GetAuthorizedUserAsync("invalid-guid", false);
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAuthorizedUserAsync_ReturnsNull_ForUserNotFound()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
        var service = new AuthService("supersecretkey", _userRepoMock.Object);

        var user = await service.GetAuthorizedUserAsync(Guid.NewGuid().ToString(), false);
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAuthorizedUserAsync_ReturnsNull_ForLoggedOutOrUnconfirmedUser()
    {
        var testUser = new User { IsLoggedOut = true, IsEmailConfirmed = false, IsFirstLogin = false, Email = string.Empty, PasswordHash = string.Empty };
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);

        var service = new AuthService("supersecretkey", _userRepoMock.Object);
        var user = await service.GetAuthorizedUserAsync(Guid.NewGuid().ToString(), true);
        Assert.Null(user);
    }

    [Fact]
    public async Task GetAuthorizedUserAsync_ReturnsUser_WhenValid()
    {
        var testUser = new User { IsLoggedOut = false, IsEmailConfirmed = true, IsFirstLogin = false, Email = string.Empty, PasswordHash = string.Empty };
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(testUser);

        var service = new AuthService("supersecretkey", _userRepoMock.Object);
        var user = await service.GetAuthorizedUserAsync(Guid.NewGuid().ToString(), true);

        Assert.NotNull(user);
        Assert.Equal(testUser, user);
    }
}

