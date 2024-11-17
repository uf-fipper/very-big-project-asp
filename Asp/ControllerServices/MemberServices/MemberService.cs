using System.Security.Cryptography;
using System.Text;
using Asp.DataModels.Members;
using Asp.DataModels.Request.Members;
using Microsoft.EntityFrameworkCore;
using Models.Context;
using Models.Models;

namespace Asp.ControllerServices.MemberControllerServices;

public class MemberService(ILogger<MemberService> logger, DatabaseContext context)
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

    public async Task<ResMember?> GetMemberFromToken(string token)
    {
        var memberToken = await context
            .MemberTokens.Include(memberToken => memberToken.Member)
            .FirstOrDefaultAsync(x => x.Token == token);
        var member = memberToken?.Member;
        return member;
    }

    public async Task<ResMember?> GetMemberFromMemcode(string memcode)
    {
        var member = await context.Members.FirstOrDefaultAsync(x => x.Memcode == memcode);
        return member;
    }

    public async Task<ResMember> Register(ReqRegister args)
    {
        string username = args.Username;
        string password = args.Password;
        if (username.Length is < 6 or > 20)
            throw new ArgumentException("Username must be between 6 and 20 characters.");
        if (password.Length < 6)
            throw new ArgumentException("password must be more than 6 characters.");

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
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw;
        }

        return member;
    }

    public async Task<ResMember?> Login(ReqLogin args)
    {
        string username = args.Username;
        string password = HashPassword(args.Password);
        var member = await context
            .Members.Where(x => x.Username == username && x.Password == password)
            .FirstOrDefaultAsync();
        if (member == null)
            return null;
        var token = GenerateToken();
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
        return resMember;
    }
}
