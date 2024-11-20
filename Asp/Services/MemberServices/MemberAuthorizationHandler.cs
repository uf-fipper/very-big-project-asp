using System.Security.Claims;
using Asp.Services.Attributes;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;

namespace Asp.Services.MemberServices;

// TODO: 还没做完
[Service(Lifetime = ServiceLifetime.Singleton)]
public class MemberAuthorizationHandler(IDatabase redis)
    : AuthorizationHandler<MemberAuthorizeAttribute>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MemberAuthorizeAttribute requirement
    ) { }
}

public class MemberPolicyProvider : IAuthorizationPolicyProvider
{
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        throw new NotImplementedException();
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
