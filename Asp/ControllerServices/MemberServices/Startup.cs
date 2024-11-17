namespace Asp.ControllerServices.MemberControllerServices;

public static class Startup
{
    public static void AddServices(IServiceCollection service)
    {
        service.AddScoped<MemberService>();
    }
}
