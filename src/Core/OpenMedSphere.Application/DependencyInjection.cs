using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using OpenMedSphere.Application.Messaging;

namespace OpenMedSphere.Application;

/// <summary>
/// Extension methods for registering Application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        RegisterHandlers(services, typeof(DependencyInjection).Assembly);

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        Type[] handlerInterfaceTypes =
        [
            typeof(ICommandHandler<>),
            typeof(ICommandHandler<,>),
            typeof(IQueryHandler<,>),
            typeof(IValidator<>)
        ];

        IEnumerable<Type> concreteTypes = assembly
            .GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false });

        foreach (Type concreteType in concreteTypes)
        {
            foreach (Type interfaceType in concreteType.GetInterfaces())
            {
                if (!interfaceType.IsGenericType)
                {
                    continue;
                }

                Type genericDefinition = interfaceType.GetGenericTypeDefinition();

                if (handlerInterfaceTypes.Contains(genericDefinition))
                {
                    services.AddScoped(interfaceType, concreteType);
                }
            }
        }
    }
}
