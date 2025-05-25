using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Akc.Framework.Mediator;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers the mediator services in the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to register the mediator services with.</param>
    /// <returns>A <c ref="MediatorBuilder"/> instance.</returns>
    public static MediatorBuilder AddMediator(this IServiceCollection services)
    {
        services.AddTransient<ISender, Sender>()
            .AddSingleton<HandlerMethodCache>()
            .AddSingleton<HandlerTypeCache>();

        return new(services);
    }

    /// <summary>
    /// Registers the mediator handlers from the specified assembly.
    /// </summary>
    /// <param name="builder">The service collection to register the handlers with.</param>
    /// <param name="assembly">The assembly containing the handler classes.</param>
    /// <returns>The updated service collection.</returns>
    public static MediatorBuilder RegisterMediatorHandlers(this MediatorBuilder builder, Assembly assembly)
    {
        builder.Services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
            .WithScopedLifetime());

        builder.Services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
            .WithScopedLifetime());

        return builder;
    }
}

/// <summary>
/// A builder class for configuring the mediator services.
/// </summary>
public sealed class MediatorBuilder
{
    public IServiceCollection Services { get; }

    public MediatorBuilder(IServiceCollection services)
    {
        Services = services;
    }
}