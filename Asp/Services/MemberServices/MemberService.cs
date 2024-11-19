using System.Security.Cryptography;
using System.Text;
using Asp.Models.Requests.Members;
using Asp.Models.Responses;
using Asp.Models.Responses.Members;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;
using Models.ModelsExt;
using StackExchange.Redis;

namespace Asp.Services.MemberServices;

[Service]
public class MemberService(ILogger<MemberService> logger, DatabaseContext context, IDatabase redis)
{
    private readonly char[] _randomChars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public string HashPassword(string raw)
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

    public string GenerateToken() => Utils.Utils.GenerateRandomString(64);

    public async Task<Result> GetMemberFromToken(string token)
    {
        var memberToken = await context
            .MemberTokens.WhereToken(token)
            .Include(mt => mt.Member)
            .FirstOrDefaultAsync();
        var member = memberToken?.Member;
        if (member == null)
        {
            return Result.Error("用户不存在");
        }

        var now = DateTime.Now;
        memberToken!.LastUseTime = now;
        memberToken!.ExpireTime = now + TimeSpan.FromDays(3);
        await context.SaveChangesAsync();
        return Result.Success<ResMember>(member);
    }

    public async Task<Result> Register(string username, string password)
    {
        var nowMember = await context.Members.FirstOrDefaultAsync(x => x.Username == username);
        if (nowMember != null)
        {
            return Result.Error("用户已存在", "USER_EXISTS");
        }

        // get member code
        string memcode = Utils.Utils.GenerateRandomString(16);
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

    public async Task<Result> Login(string username, string password)
    {
        password = HashPassword(password);
        var member = await context
            .Members.Where(x => x.Username == username && x.Password == password)
            .FirstOrDefaultAsync();
        if (member == null)
            return Result.Error("用户名或密码错误");

        var memberToken = await context.MemberTokens.MustAvailable().FirstOrDefaultAsync();
        bool isUpdate;

        string token = GenerateToken();
        DateTime now = DateTime.Now;
        if (memberToken == null)
        {
            memberToken = new MemberToken
            {
                MemberId = member.Id,
                Status = 1,
                Token = token,
                LastUseTime = now,
                LastLoginTime = now,
                ExpireTime = now + TimeSpan.FromDays(3),
            };
            await context.AddAsync(memberToken);
            isUpdate = false;
        }
        else
        {
            memberToken.Token = token;
            isUpdate = true;
        }

        await context.SaveChangesAsync();
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
}
