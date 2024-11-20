namespace Asp.Services.ConstantsServices;

public partial class Constants
{
    /// <summary>
    /// token的前缀
    /// </summary>
    public const string MemberTokenPrefix = "MEMBER_TOKEN";

    public static string GetMemberKeyFromToken(string token) => $"{MemberTokenPrefix}:{token}";

    /// <summary>
    /// 锁的前缀
    /// </summary>
    public const string LockPrefix = "REDIS_LOCK";

    public static string GetLockKey(string key) => $"{LockPrefix}:{key}";
}
