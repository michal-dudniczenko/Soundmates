using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soundmates.Domain.Entities;
using Soundmates.Infrastructure.Identity;

namespace Soundmates.Infrastructure.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public DbSet<MusicSample> MusicSamples { get; set; }
    public DbSet<ProfilePicture> ProfilePictures { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Dislike> Dislikes { get; set; }
    public DbSet<Match> Matches { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<MusicSample>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ProfilePicture>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Like>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.GiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Like>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Dislike>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.GiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Dislike>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Match>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Like>()
            .HasIndex(e => new { e.GiverId, e.ReceiverId })
            .IsUnique();

        builder.Entity<Dislike>()
            .HasIndex(e => new { e.GiverId, e.ReceiverId })
            .IsUnique();

        builder.Entity<Match>()
            .HasIndex(e => new { e.User1Id, e.User2Id })
            .IsUnique();
    }
}
