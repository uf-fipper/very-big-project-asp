namespace Asp.ControllerServices;

public static class Startup
{
    public static IServiceCollection AddControllerServices(this IServiceCollection service)
    {
        MemberControllerServices.Startup.AddServices(service);
        return service;
    }
}
