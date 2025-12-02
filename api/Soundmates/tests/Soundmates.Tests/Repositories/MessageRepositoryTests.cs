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

public class MessageRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public MessageRepositoryTests()
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
    private MessageRepository CreateRepo(ApplicationDbContext ctx) => new MessageRepository(ctx);

    private User CreateUser(Guid id)
    {
        var country = new Country { Name = string.Empty };
        var city = new City { Latitude = 0d, Longitude = 0d, Name = string.Empty, Country = country };

        return new User { Id = id, Email = string.Empty, PasswordHash = string.Empty, Country = country, City = city };
    }

    private Message CreateMessage(Guid sender, Guid receiver, int ts, bool seen = false)
    {
        return new Message
        {
            Id = Guid.NewGuid(),
            SenderId = sender,
            ReceiverId = receiver,
            Content = string.Empty,
            IsSeen = seen
        };
    }

    // =====================================================================
    // AddAsync
    // =====================================================================
    [Fact]
    public async Task AddAsync_AddsMessage()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var u1 = CreateUser(Guid.NewGuid());
        var u2 = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(u1, u2);
        await ctx.SaveChangesAsync();

        var msg = CreateMessage(u1.Id, u2.Id, 0);

        await repo.AddAsync(msg);

        Assert.Single(ctx.Messages);
    }

    // =====================================================================
    // GetConversationAsync
    // =====================================================================
    [Fact]
    public async Task GetConversationAsync_ReturnsMessagesInCorrectOrder()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var u1 = CreateUser(Guid.NewGuid());
        var u2 = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(u1, u2);
        await ctx.SaveChangesAsync();

        ctx.Messages.AddRange(
            CreateMessage(u1.Id, u2.Id, -2),
            CreateMessage(u2.Id, u1.Id, -1),
            CreateMessage(u1.Id, u2.Id, -3)
        );

        await ctx.SaveChangesAsync();

        var result = await repo.GetConversationAsync(u1.Id, u2.Id, 10, 0);

        Assert.Equal(3, result.Count());
        Assert.True(result.First().Timestamp < result.Last().Timestamp);
    }

    [Fact]
    public async Task GetConversationAsync_RespectsPagination()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var u1 = CreateUser(Guid.NewGuid());
        var u2 = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(u1, u2);
        await ctx.SaveChangesAsync();

        ctx.Messages.AddRange(
            CreateMessage(u1.Id, u2.Id, -3),
            CreateMessage(u1.Id, u2.Id, -2),
            CreateMessage(u1.Id, u2.Id, -1)
        );
        await ctx.SaveChangesAsync();

        var page = await repo.GetConversationAsync(u1.Id, u2.Id, 1, 1);

        Assert.Single(page);
    }

    // =====================================================================
    // GetConversationsPreviewAsync
    // =====================================================================
    [Fact]
    public async Task GetConversationsPreviewAsync_ReturnsLatestMessageFromEachConversation()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var self = CreateUser(Guid.NewGuid());
        var other1 = CreateUser(Guid.NewGuid());
        var other2 = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(self, other1, other2);
        await ctx.SaveChangesAsync();

        ctx.Messages.AddRange(
            CreateMessage(self.Id, other1.Id, -5),
            CreateMessage(other1.Id, self.Id, -1), // latest for conversation 1
            CreateMessage(self.Id, other2.Id, -3),
            CreateMessage(other2.Id, self.Id, -2), // latest for conversation 2
            CreateMessage(other2.Id, self.Id, -10) // ignored old one
        );

        await ctx.SaveChangesAsync();

        var previews = (await repo.GetConversationsPreviewAsync(self.Id)).ToList();

        Assert.Equal(2, previews.Count);
        Assert.True(previews[0].Timestamp > previews[1].Timestamp);
    }

    [Fact]
    public async Task GetConversationsPreviewAsync_ReturnsEmpty_WhenNoMessages()
    {
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var user = CreateUser(Guid.NewGuid());
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var previews = await repo.GetConversationsPreviewAsync(user.Id);

        Assert.Empty(previews);
    }

    // =====================================================================
    // ReadConversation
    // =====================================================================
    [Fact]
    public async Task ReadConversation_MarksOnlyUnseenMessagesAsSeen()
    {
        // Arrange
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var self = CreateUser(Guid.NewGuid());
        var other = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(self, other);
        await ctx.SaveChangesAsync();

        // Messages: one seen, one unseen
        var seenMessage = CreateMessage(other.Id, self.Id, -2, seen: true);
        var unseenMessage = CreateMessage(other.Id, self.Id, -1, seen: false);

        ctx.Messages.AddRange(seenMessage, unseenMessage);
        await ctx.SaveChangesAsync();

        // Act
        await repo.ReadConversation(self.Id, other.Id);

        // Assert
        var messages = await ctx.Messages.AsNoTracking().ToListAsync();

        var seenMsg = messages.Single(m => m.Id == seenMessage.Id);
        var unseenMsg = messages.Single(m => m.Id == unseenMessage.Id);

        Assert.True(seenMsg.IsSeen, "Already seen message should stay seen");
        Assert.True(unseenMsg.IsSeen, "Previously unseen message should now be marked as seen");
    }


    [Fact]
    public async Task ReadConversation_MarksUnseenMessagesAsSeen_LeavesAlreadySeenMessagesIntact()
    {
        // Arrange
        using var ctx = CreateDb();
        var repo = CreateRepo(ctx);

        var self = CreateUser(Guid.NewGuid());
        var other = CreateUser(Guid.NewGuid());

        ctx.Users.AddRange(self, other);
        await ctx.SaveChangesAsync();

        // Create messages: one already seen, one not seen
        var seenMessage = CreateMessage(other.Id, self.Id, ts: -2, seen: true);
        var unseenMessage = CreateMessage(other.Id, self.Id, ts: -1, seen: false);

        ctx.Messages.AddRange(seenMessage, unseenMessage);
        await ctx.SaveChangesAsync();

        // Act
        await repo.ReadConversation(self.Id, other.Id);

        // Reload messages from context to get updated state
        ctx.Entry(seenMessage).Reload();
        ctx.Entry(unseenMessage).Reload();

        // Assert
        Assert.True(seenMessage.IsSeen, "Already seen message should remain seen.");
        Assert.True(unseenMessage.IsSeen, "Previously unseen message should now be marked as seen.");
    }

}

