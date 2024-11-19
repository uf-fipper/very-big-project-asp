using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Asp.Services;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public Type? ServiceType { get; set; }

    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

    public virtual void TryAddService(WebApplicationBuilder builder, Type type)
    {
        builder.Services.TryAdd(ServiceDescriptor.Describe(ServiceType ?? type, type, Lifetime));
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ServiceKeyedAttribute(object key) : ServiceAttribute
{
    public object Key { get; set; } = key;

    public override void TryAddService(WebApplicationBuilder builder, Type type)
    {
        builder.Services.TryAdd(
            ServiceDescriptor.DescribeKeyed(ServiceType ?? type, Key, type, Lifetime)
        );
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationServiceAttribute(string key) : ServiceAttribute
{
    public string? Key { get; set; } = key;

    private static readonly MethodInfo ConfigureMethod = typeof(IServiceCollection)
        .GetMethods(BindingFlags.Public)
        .First(x =>
            x.GetParameters().Length == 2
            && x.GetParameters()[0].ParameterType == typeof(string)
            && x.GetParameters()[1].ParameterType == typeof(IConfiguration)
        );

    public override void TryAddService(WebApplicationBuilder builder, Type type)
    {
        var method = ConfigureMethod.MakeGenericMethod(type);
        var key = Key ?? type.Name;
        var config = builder.Configuration.GetSection(key);
        method.Invoke(builder.Services, [key, config]);
        builder.Services.Configure<Type>("", config);
    }
}
