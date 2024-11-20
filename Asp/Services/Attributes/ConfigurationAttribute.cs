using System.Reflection;

namespace Asp.Services.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : BaseServiceAttribute
{
    public string? Key { get; set; }

    public string? Name { get; set; }

    private static readonly MethodInfo ConfigureMethod =
        typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod(
            "Configure",
            BindingFlags.Public | BindingFlags.Static,
            null,
            CallingConventions.Any,
            [typeof(IServiceCollection), typeof(string), typeof(IConfiguration)],
            null
        )!;

    private static readonly MethodInfo ConfigureWithNameMethod =
        typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod(
            "Configure",
            BindingFlags.Public | BindingFlags.Static,
            null,
            CallingConventions.Any,
            [typeof(IServiceCollection), typeof(string), typeof(IConfiguration)],
            null
        )!;

    public override void ExtraBuild(WebApplicationBuilder builder, Type type)
    {
        var config = builder.Configuration.GetSection(Key ?? type.Name);
        if (Name == null)
            ConfigureMethod.MakeGenericMethod(type).Invoke(null, [builder.Services, config]);
        else
            ConfigureWithNameMethod
                .MakeGenericMethod(type)
                .Invoke(null, [builder.Services, Name, config]);
    }
}
