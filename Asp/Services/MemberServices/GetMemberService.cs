using Asp.Services.Attributes;
using Asp.Services.ConstantsServices;
using Asp.Utils;
using Microsoft.Extensions.Primitives;
using Models.Models;
using StackExchange.Redis;

namespace Asp.Services.MemberServices;

// [Service]
public class GetMemberService(HttpContext context, IDatabase redis)
{
    public Member? Value
    {
        get
        {
            context.Items.TryGetValue("token", out object? value);
            return value as Member;
        }
    }
}

[Middleware]
public class GetMemberMiddleware(RequestDelegate next, IDatabase redis)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue("token", out StringValues value);
        string? token = value;
        Member? member = null;
        if (token != null)
        {
            string redisKey = Constants.GetMemberKeyFromToken(token);
            member = await redis.ObjectGetAsync<Member>(redisKey);
        }

        context.Items["Member"] = member;

        await next(context);
    }
}
