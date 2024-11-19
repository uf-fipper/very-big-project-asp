﻿using System.ComponentModel.DataAnnotations;
using Asp.Models.Requests.Members;
using Asp.Models.Responses;
using Asp.Models.Responses.Members;
using Asp.Services.MemberServices;
using Microsoft.AspNetCore.Mvc;
using Models.Context;
using Swashbuckle.AspNetCore.Annotations;

namespace Asp.Controllers.MemberControllers;

[ApiController]
[Route("[controller]/[action]")]
public class MemberController(
    ILogger<MemberController> logger,
    DatabaseContext context,
    MemberService memberService
) : Controller
{
    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="token">用户token</param>
    /// <returns></returns>
    /// <remarks>
    /// 这是remarks
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(ResultSuccess<ResMember>), 200)]
    [ProducesResponseType(typeof(ResultError<string>), 401)]
    public async Task<IActionResult> GetMember([FromHeader, Required] string token)
    {
        var result = await memberService.GetMemberFromToken(token);
        if (result is ResultSuccess<ResMember>)
            return Ok(result);
        return Unauthorized(result);
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerResponse(200, "用户已存在", typeof(ResultError<string>))]
    [SwaggerResponse(200, "注册成功", typeof(ResultSuccess<ResMember>))]
    public async Task<IActionResult> Register([FromBody, Required] ReqRegister args)
    {
        var result = await memberService.Register(args.Username, args.Password);
        return Ok(result);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerResponse(200, "登录成功", typeof(ResultSuccess<ResLoginMember>))]
    public async Task<IActionResult> Login([FromBody, Required] ReqLogin args)
    {
        var result = await memberService.Login(args.Username, args.Password);
        return Ok(result);
    }
}