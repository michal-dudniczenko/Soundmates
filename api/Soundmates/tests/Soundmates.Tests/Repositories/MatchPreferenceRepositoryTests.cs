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

public class MatchPreferenceRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public MatchPreferenceRepositoryTests()
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
    private MatchPreferenceRepository CreateRepo(ApplicationDbContext ctx)
        => new MatchPreferenceRepository(ctx);

    // -------------------------------------------------------------
    // AddUpdateAsync — NEW
    // -------------------------------------------------------------
    [Fact]
    public async Task AddUpdateAsync_AddsNew_WhenNotExists_WithoutTags()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };


        var entity = new UserMatchPreference
        {
            UserId = userId,
            User = user,
            ShowArtists = true,
            ShowBands = false
        };

        await repo.AddUpdateAsync(entity);

        var saved = await ctx.UserMatchPreferences.FirstOrDefaultAsync();
        Assert.NotNull(saved);
        Assert.Equal(userId, saved!.UserId);
        Assert.Empty(saved.Tags);
    }

    [Fact]
    public async Task AddUpdateAsync_AddsNew_WithValidTags()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var tag1 = new Tag
        {
            Name = "T1",
            TagCategory = new TagCategory { Name = "C", IsForBand = false }
        };

        ctx.Tags.Add(tag1);
        await ctx.SaveChangesAsync();

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };

        var entity = new UserMatchPreference
        {
            UserId = userId,
            User = user,
            ShowArtists = true
        };

        await repo.AddUpdateAsync(entity, new List<Guid> { tag1.Id });

        var saved = await ctx.UserMatchPreferences.Include(p => p.Tags).FirstAsync();

        Assert.Single(saved.Tags);
        Assert.Equal(tag1.Id, saved.Tags.First().Id);
    }

    [Fact]
    public async Task AddUpdateAsync_Throws_WhenInvalidTagProvided()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var entity = new UserMatchPreference
        {
            UserId = Guid.NewGuid(),
            ShowArtists = true
        };

        var invalidId = Guid.NewGuid(); // does not exist

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repo.AddUpdateAsync(entity, new List<Guid> { invalidId })
        );

        Assert.Contains("Invalid tag id provided", ex.Message);
    }

    // -------------------------------------------------------------
    // AddUpdateAsync — UPDATE
    // -------------------------------------------------------------
    [Fact]
    public async Task AddUpdateAsync_UpdatesExisting_AndReplacesTags()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };


        // Tag categories for required FK
        var cat = new TagCategory { Name = "C", IsForBand = false };
        ctx.TagCategories.Add(cat);

        var originalTag = new Tag { Id = Guid.NewGuid(), Name = "Old", TagCategoryId = cat.Id };
        var newTag = new Tag { Id = Guid.NewGuid(), Name = "New", TagCategoryId = cat.Id };

        ctx.Tags.AddRange(originalTag, newTag);

        var existing = new UserMatchPreference
        {
            User = user,
            ShowArtists = false,
            ShowBands = false
        };

        existing.Tags.Add(originalTag);

        ctx.UserMatchPreferences.Add(existing);
        await ctx.SaveChangesAsync();

        var updated = new UserMatchPreference
        {
            User = user,
            ShowArtists = true,
            ShowBands = true
        };

        await repo.AddUpdateAsync(updated, new List<Guid> { newTag.Id });

        var saved = await ctx.UserMatchPreferences.Include(mp => mp.Tags).FirstAsync();

        Assert.True(saved.ShowArtists);
        Assert.True(saved.ShowBands);
        Assert.Single(saved.Tags);
        Assert.Equal(newTag.Id, saved.Tags.First().Id);
    }

    // -------------------------------------------------------------
    // GetUserMatchPreferenceAsync
    // -------------------------------------------------------------
    [Fact]
    public async Task GetUserMatchPreferenceAsync_ReturnsExisting()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };
        var gender = new Gender { Name = string.Empty };

        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };


        var pref = new UserMatchPreference { User = user };
        ctx.UserMatchPreferences.Add(pref);

        await ctx.SaveChangesAsync();

        var result = await repo.GetUserMatchPreferenceAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.UserId);
    }

    [Fact]
    public async Task GetUserMatchPreferenceAsync_ReturnsNull_WhenNotFound()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var result = await repo.GetUserMatchPreferenceAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}

