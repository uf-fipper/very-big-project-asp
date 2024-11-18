using System.Security.Cryptography;
using System.Text;
using Asp.Models.Requests.Members;
using Asp.Models.Responses;
using Asp.Models.Responses.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Models.Context;
using Models.Models;
using MySqlConnector;
using StackExchange.Redis;

namespace Asp.ControllerServices.MemberControllerServices;

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
        var memberToken = await MemberToken.FromToken(
            context,
            token,
            q => q.Include(mt => mt.Member)
        );
        var member = memberToken?.Member;
        if (member == null)
        {
            return Result.Error("用户不存在");
        }
        return Result.Success<ResMember>(member);
    }

    public async Task<Result> Register(ReqRegister args)
    {
        string username = args.Username;
        string password = args.Password;

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

    public async Task<Result> Login(ReqLogin args)
    {
        string username = args.Username;
        string password = HashPassword(args.Password);
        var member = await context
            .Members.Where(x => x.Username == username && x.Password == password)
            .FirstOrDefaultAsync();
        if (member == null)
            return Result.Error("用户名或密码错误");
        string token = GenerateToken();
        DateTime now = DateTime.Now;
        var memberToken = new MemberToken
        {
            MemberId = member.Id,
            Status = 1,
            Token = token,
            LastUseTime = now,
            LastLoginTime = now,
        };
        await context.MemberTokens.AddAsync(memberToken);
        ResMember resMember = member;
        resMember.Token = token;
        return Result.Success(resMember);
    }
}
