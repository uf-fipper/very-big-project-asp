using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Asp.ControllerServices;

public static class Startup
{
    public static IServiceCollection TryAddServices(this IServiceCollection service)
    {
        var types = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a =>
                a.GetTypes().Where(t => t.GetCustomAttribute(typeof(ServiceAttribute)) != null)
            )
            .ToList();
        foreach (var type in types)
        {
            var attribute = (ServiceAttribute)type.GetCustomAttribute(typeof(ServiceAttribute))!;
            service.TryAdd(
                ServiceDescriptor.Describe(attribute.ServiceType ?? type, type, attribute.Lifetime)
            );
        }
        return service;
    }
}
