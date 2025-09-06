using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;
using Soundmates.Domain.Interfaces.Services.Mp3;
using Soundmates.Infrastructure.Database;
using Soundmates.Infrastructure.Repositories;
using Soundmates.Infrastructure.Services.Auth;
using Soundmates.Infrastructure.Services.Mp3;

namespace Soundmates.Infrastructure.Extensions;

public static class DependencyInjection
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
            options.UseNpgsql(connectionString);
        });

        var secretKey = configuration["SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Secret key is not configured.");
        }

        services.AddScoped<IDislikeRepository, DislikeRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
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

        services.AddSingleton<IMp3Service>(new Mp3Service());

        return services;
    }
}
