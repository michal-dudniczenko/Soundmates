using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using Soundmates.Infrastructure.Services.Auth;
using static Soundmates.Infrastructure.Database.DataSeeding.SeedingScripts;

namespace Soundmates.Infrastructure;

public static class DIRegistrations
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseAsyncSeeding(async (context, _, ct) =>
                {
                    if (!await context.Set<Country>().AnyAsync(cancellationToken: ct))
                    {
                        await SeedData(context, ct);
                    }
                });
        });

        var secretKey = configuration["SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Secret key is not configured.");
        }

        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IBandRepository, BandRepository>();
        services.AddScoped<IDictionaryRepository, DictionaryRepository>();
        services.AddScoped<IMatchPreferenceRepository, MatchPreferenceRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMusicSampleRepository, MusicSampleRepository>();
        services.AddScoped<IProfilePictureRepository, ProfilePictureRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IAuthService>(sp =>
        {
            var userRepository = sp.GetRequiredService<IUserRepository>();
            return new AuthService(secretKey, userRepository);
        });

        services.AddScoped<Soundmates.Domain.Interfaces.Services.IEmailService, Soundmates.Infrastructure.Services.EmailService>();

        services.AddSignalR();

        return services;
    }
}
