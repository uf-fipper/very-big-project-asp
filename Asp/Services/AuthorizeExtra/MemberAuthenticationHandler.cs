using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
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
        await Task.CompletedTask;

        var metaData = Context.GetEndpoint()?.Metadata;

        // 存在 AllowAnonymousAttribute 则不鉴权
        if (metaData?.GetMetadata<AllowAnonymousAttribute>() != null)
            return AuthenticateResult.NoResult();

        // 不存在 AuthorizeAttribute 则不鉴权
        if (metaData?.GetMetadata<AuthorizeAttribute>() == null)
            return AuthenticateResult.NoResult();

        string? token = Request.Headers["token"];

        if (token == null)
            return AuthenticateResult.NoResult();
        string? memberJson = await redis.StringGetAsync(Constants.GetMemberKeyFromToken(token));
        if (memberJson == null)
            return AuthenticateResult.Fail("请登录");
        Member? member = JsonSerializer.Deserialize<Member>(memberJson);
        if (member == null)
            return AuthenticateResult.Fail("请登录");

        ClaimsIdentity claims = new();
        claims.AddClaim(new Claim("member", memberJson));
        claims.AddClaim(new Claim(ClaimTypes.Role, "Admin"));

        ClaimsPrincipal principal = new(claims);

        AuthenticationTicket ticket = new(principal, "Member");

        return AuthenticateResult.Success(ticket);
    }
}

public class MemberAuthorizationHandler : AuthorizationHandler<MemberAuthorizeAttribute>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MemberAuthorizeAttribute requirement
    )
    {
        await Task.CompletedTask;
        if (context.Resource is not DefaultHttpContext httpContext)
            return;
    }
}

public class MemberAuthorizeAttribute
    : AuthorizeAttribute,
        IAuthorizationRequirement,
        IAuthorizationRequirementData
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
