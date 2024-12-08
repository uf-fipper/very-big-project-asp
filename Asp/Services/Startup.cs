using System.Reflection;
using Asp.Services.Attributes;
using Asp.Services.MemberServices;

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

    public static WebApplication UseMemberMiddleware(this WebApplication app)
    {
        app.UseMiddleware<GetMemberMiddleware>();

        return app;
    }
}
