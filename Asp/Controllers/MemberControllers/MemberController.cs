using System.ComponentModel.DataAnnotations;
using Asp.ControllerServices.MemberControllerServices;
using Asp.DataModels;
using Asp.DataModels.Members;
using Asp.DataModels.Request.Members;
using Microsoft.AspNetCore.Mvc;
using Models.Context;

namespace Asp.Controllers.MemberControllers;

[Route("member")]
public class MemberController(
    ILogger<MemberController> logger,
    DatabaseContext context,
    MemberService memberService
) : Controller
{
    [HttpGet("getMember")]
    public async Task<IActionResult> GetMember([FromHeader, Required] string token)
    {
        ResMember? member = await memberService.GetMemberFromToken(token);
        if (member == null)
            return Unauthorized(Result.Error("用户不存在"));
        return Ok(Result.Success(member));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody, Required] ReqRegister args)
    {
        if (args.Username.Length is < 6 or > 20)
            return Ok(Result.Error("用户昵称必须在6到20个字符之间"));
        if (args.Password.Length < 6)
            return Ok(Result.Error("密码长度不能低于6个字符"));
        ResMember member = await memberService.Register(args);
        return Ok(Result.Success(member));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody, Required] ReqLogin args)
    {
        ResMember? member = await memberService.Login(args);
        if (member == null)
            return Ok(Result.Error("用户不存在"));
        return Ok(Result.Success("用户登陆成功"));
    }
}
