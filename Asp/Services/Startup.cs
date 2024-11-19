using System.Reflection;

namespace Asp.Services;

public static class Startup
{
    public static WebApplicationBuilder TryAddServices(this WebApplicationBuilder builder)
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
            attribute.TryAddService(builder, type);
        }

        return builder;
    }
}
