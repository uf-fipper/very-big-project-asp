using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Asp.Services.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseServiceAttribute : BaseStartupAttribute { }

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : BaseServiceAttribute
{
    public Type? ServiceType { get; set; }

    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

    public override void ExtraBuild(WebApplicationBuilder builder, Type type)
    {
        builder.Services.TryAdd(ServiceDescriptor.Describe(ServiceType ?? type, type, Lifetime));
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ServiceWithKeyAttribute(object key) : ServiceAttribute
{
    public object Key { get; set; } = key;

    public override void ExtraBuild(WebApplicationBuilder builder, Type type)
    {
        builder.Services.TryAdd(
            ServiceDescriptor.DescribeKeyed(ServiceType ?? type, Key, type, Lifetime)
        );
    }
}
