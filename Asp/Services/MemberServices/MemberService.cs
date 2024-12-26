using System.Security.Cryptography;
using System.Text;
using Asp.Models.Requests.Members;
using Asp.Models.Responses;
using Asp.Models.Responses.Members;
using Asp.Services.Attributes;
using Asp.Services.ConstantsServices;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Models.ModelsExt;
using StackExchange.Redis;
using Utils.RedisUtils;
using Utils.StringUtils;

namespace Asp.Services.MemberServices;

[Service]
public class MemberService(ILogger<MemberService> logger, DatabaseContext context, IDatabase redis)
{
    /// <summary>
    /// 密码哈希方法
    /// </summary>
    /// <param name="raw">原始密码</param>
    /// <returns>哈希后的字符串</returns>
    private static string HashPassword(string raw)
    {
        // 加盐
        string text = $"very-big{raw}project";

        // sha256 加密一次
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // 计算哈希值
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            // 转换字节数组为十六进制字符串
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // 转为小写十六进制
            }

            text = builder.ToString();
        }

        // 再加盐
        text = $"very-big{text}-project";

        // sha512 再加密一次
        using (SHA512 sha512Hash = SHA512.Create())
        {
            // 计算哈希值
            byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            // 转换字节数组为十六进制字符串
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2")); // 转为小写十六进制
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// 生成token
    /// </summary>
    /// <returns>token</returns>
    private static string GenerateToken() => StringUtils.GenerateRandomString(64);

    /// <summary>
    /// 从token获取member
    /// </summary>
    /// <param name="token">token</param>
    /// <returns></returns>
    public async Task<Member?> GetMemberFromToken(string token)
    {
        string redisKey = Constants.GetMemberKeyFromToken(token);
        Member? member = await redis.ObjectGetAsync<Member>(redisKey);
        if (member == null)
            return null;

        await redis.ObjectSetAsync(redisKey, member, TimeSpan.FromDays(3));
        return member;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public async Task<Result> Register(string username, string password)
    {
        var nowMember = await context.Members.FirstOrDefaultAsync(x => x.Username == username);
        if (nowMember != null)
        {
            return Result.Error("用户已存在", "USER_EXISTS");
        }

        // get member code
        string memcode = StringUtils.GenerateRandomString(16);
        var member = new Member
        {
            Memcode = memcode,
            Username = username,
            Password = HashPassword(password),
            Nickname = "大佬！",
        };
        await context.Members.AddAsync(member);
        await context.SaveChangesAsync();

        return Result.Success<ResMember>(member);
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    /// <returns></returns>
    public async Task<Result> Login(string username, string password)
    {
        await using var redisLock = await redis.LockAsync(
            Constants.GetLockKey(username),
            TimeSpan.FromSeconds(30)
        );
        if (!redisLock.Locked)
            return Result.Error("操作太快，请稍后重试", "TOO_FAST");
        password = HashPassword(password);
        var member = await context
            .Members.Where(x => x.Username == username && x.Password == password)
            .Include(m => m.MemberTokens.Take(1))
            .FirstOrDefaultAsync();
        if (member == null)
            return Result.Error("用户名或密码错误");

        var memberToken = member.MemberTokens.FirstOrDefault();
        bool isUpdate;

        string? oldToken = null;
        string token = GenerateToken();
        DateTime now = DateTime.Now;
        if (memberToken == null)
        {
            memberToken = new MemberToken
            {
                MemberId = member.Id,
                Token = token,
                LastLoginTime = now,
            };
            await context.AddAsync(memberToken);
            isUpdate = false;
        }
        else
        {
            oldToken = memberToken.Token;
            memberToken.Token = token;
            memberToken.LastLoginTime = now;
            isUpdate = true;
        }

        await context.SaveChangesAsync();
        string redisKey = Constants.GetMemberKeyFromToken(token);
        if (oldToken != null)
            await Logout(oldToken);
        var oldMemberTokens = member.MemberTokens;
        member.MemberTokens = [];
        await redis.ObjectSetAsync(redisKey, member, TimeSpan.FromDays(3));
        member.MemberTokens = oldMemberTokens;
        ResLoginMember resMember = new ResLoginMember
        {
            IsUpdate = isUpdate,
            Memcode = member.Memcode,
            Nickname = member.Nickname,
            Token = token,
            Username = member.Username,
        };
        return Result.Success(resMember);
    }

    public async Task<Result> Logout(string token)
    {
        string redisKey = Constants.GetMemberKeyFromToken(token);
        bool isDelete = await redis.KeyDeleteAsync(redisKey);
        return Result.Success(isDelete);
    }
}
