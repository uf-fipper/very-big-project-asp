using System.Reflection;
using Asp.Services.Attributes;

namespace Asp.Services;

public static class Startup
{
    public static WebApplicationBuilder ExtraBuild(this WebApplicationBuilder builder)
    {
        var types = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a =>
                a.GetTypes().Where(t => t.GetCustomAttribute(typeof(BaseStartupAttribute)) != null)
            )
            .ToList();
        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes<BaseStartupAttribute>();
            foreach (var attribute in attributes)
            {
                attribute.ExtraBuild(builder, type);
            }
        }

        return builder;
    }

    public static WebApplication ExtraUse(this WebApplication app)
    {
        var types = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a =>
                a.GetTypes().Where(t => t.GetCustomAttribute(typeof(BaseStartupAttribute)) != null)
            )
            .ToList();
        foreach (var type in types)
        {
            var attributes = type.GetCustomAttributes<BaseStartupAttribute>();
            foreach (var attribute in attributes)
            {
                attribute.ExtraUse(app, type);
            }
        }

        return app;
    }
}
