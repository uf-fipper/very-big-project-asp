using System.Diagnostics.CodeAnalysis;

namespace Asp.ControllerServices;

public class ServiceAttribute : Attribute
{
    public Type? ServiceType { get; set; }

    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
}
