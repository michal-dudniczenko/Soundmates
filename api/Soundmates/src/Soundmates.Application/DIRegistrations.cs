using Microsoft.Extensions.DependencyInjection;

namespace Soundmates.Application;

public static class DIRegistrations
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(cfg =>
        {
            var applicationAssembly = typeof(DIRegistrations).Assembly;
            cfg.RegisterServicesFromAssemblies(applicationAssembly);
        });

        return services;
    }
}
