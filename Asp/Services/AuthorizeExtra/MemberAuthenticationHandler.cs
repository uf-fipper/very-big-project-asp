using System.Security.Claims;
using System.Text.Encodings.Web;
using Asp.Services.ConstantsServices;
using Asp.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Models.Models;
using StackExchange.Redis;

namespace Asp.Services.AuthorizeExtra;

public class MemberAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IDatabase redis
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.Delay(0);
        string? token = Request.Headers["token"];

        // 存在 AllowAnonymousAttribute 则不鉴权
        object? attr = Context
            .GetEndpoint()
            ?.Metadata.FirstOrDefault(x => x is AllowAnonymousAttribute);
        if (attr != null)
            return AuthenticateResult.NoResult();

        // 不存在 AuthorizeAttribute 则不鉴权
        attr = Context.GetEndpoint()?.Metadata.FirstOrDefault(x => x is AuthorizeAttribute);
        if (attr == null)
            return AuthenticateResult.NoResult();
        if (token == null)
            return AuthenticateResult.NoResult();
        var member = await redis.ObjectGetAsync<Member>(Constants.GetMemberKeyFromToken(token));
        if (member == null)
            return AuthenticateResult.Fail("请登录");
        var claims = new ClaimsIdentity();
        ClaimsPrincipal principal = new ClaimsPrincipal(claims);
        AuthenticationTicket ticket = new AuthenticationTicket(principal, "Member");
        return AuthenticateResult.Success(ticket);
    }
}
