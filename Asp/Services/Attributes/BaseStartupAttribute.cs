using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Asp.Services.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public abstract class BaseStartupAttribute : Attribute
{
    public virtual void ExtraBuild(WebApplicationBuilder builder, Type type) { }

    public virtual void ExtraUse(WebApplication app, Type type) { }
}
