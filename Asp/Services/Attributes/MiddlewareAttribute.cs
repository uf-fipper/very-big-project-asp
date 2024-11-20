namespace Asp.Services.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MiddlewareAttribute(object[]? args = null) : BaseStartupAttribute
{
    public readonly object[]? Args = args;

    public override void ExtraUse(WebApplication app, Type type)
    {
        if (Args == null)
            app.UseMiddleware(type);
        else
            app.UseMiddleware(type, Args);
    }
}
