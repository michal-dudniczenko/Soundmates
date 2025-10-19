using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;

namespace Soundmates.Infrastructure.Database;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Band> Bands { get; set; }
    public DbSet<BandMember> BandMembers { get; set; }
    public DbSet<BandRole> BandRoles { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagCategory> TagCategories { get; set; }
    public DbSet<MusicSample> MusicSamples { get; set; }
    public DbSet<ProfilePicture> ProfilePictures { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Dislike> Dislikes { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Gender> Genders { get; set; }
    public DbSet<UserMatchPreference> UserMatchPreferences { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region relationships
        modelBuilder.Entity<User>(entity =>
        {
            entity
                .HasOne(u => u.Country)
                .WithMany()
                .HasForeignKey(u => u.CountryId);

            entity
                .HasOne(u => u.City)
                .WithMany()
                .HasForeignKey(u => u.CityId);

            entity
                .HasMany(u => u.Tags)
                .WithMany();

            entity
                .HasMany(u => u.ProfilePictures)
                .WithOne(pp => pp.User)
                .HasForeignKey(pp => pp.UserId);

            entity
                .HasMany(u => u.MusicSamples)
                .WithOne(ms => ms.User)
                .HasForeignKey(ms => ms.UserId);

            entity
                .HasMany(u => u.Artists)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);

            entity
                .HasMany(u => u.Bands)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);
        });

        modelBuilder.Entity<Artist>(entity =>
        {
            entity
                .HasOne(a => a.Gender)
                .WithMany()
                .HasForeignKey(a => a.GenderId);
        });

        modelBuilder.Entity<Band>(entity =>
        {
            entity
                .HasMany(b => b.Members)
                .WithOne(bm => bm.Band)
                .HasForeignKey(bm => bm.BandId);
        });

        modelBuilder.Entity<BandMember>(entity =>
        {
            entity
                .HasOne(bm => bm.BandRole)
                .WithMany()
                .HasForeignKey(bm => bm.BandRoleId);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity
                .HasOne(t => t.TagCategory)
                .WithMany(tc => tc.Tags)
                .HasForeignKey(t => t.TagCategoryId);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId);

            entity
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity
                .HasOne(l => l.Giver)
                .WithMany()
                .HasForeignKey(l => l.GiverId);

            entity
                .HasOne(l => l.Receiver)
                .WithMany()
                .HasForeignKey(l => l.ReceiverId);
        });

        modelBuilder.Entity<Dislike>(entity =>
        {
            entity
                .HasOne(dl => dl.Giver)
                .WithMany()
                .HasForeignKey(dl => dl.GiverId);

            entity
                .HasOne(dl => dl.Receiver)
                .WithMany()
                .HasForeignKey(dl => dl.ReceiverId);
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity
                .HasOne(m => m.User1)
                .WithMany()
                .HasForeignKey(m => m.User1Id);

            entity
                .HasOne(m => m.User2)
                .WithMany()
                .HasForeignKey(m => m.User2Id);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity
                .HasOne(rt => rt.User)
                .WithOne()
                .HasForeignKey<RefreshToken>(rt => rt.UserId);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity
                .HasOne(c => c.Country)
                .WithMany()
                .HasForeignKey(c => c.CountryId);
        });

        modelBuilder.Entity<UserMatchPreference>(entity =>
        {
            entity
                .HasOne(ump => ump.User)
                .WithOne()
                .HasForeignKey<UserMatchPreference>(ump => ump.UserId);

            entity
                .HasMany(ump => ump.Tags)
                .WithMany();
        });
        #endregion
    }

}
