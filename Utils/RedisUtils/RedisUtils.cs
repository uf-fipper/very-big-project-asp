using System.Text.Json;
using System.Text.Json.Serialization;
using StackExchange.Redis;

namespace Utils.RedisUtils;

public static class RedisUtils
{
    private static readonly JsonSerializerOptions IgnoreWriteDefaultOption =
        new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

    /// <summary>
    /// 将json对象取出，解析成类
    /// </summary>
    /// <param name="redis"></param>
    /// <param name="key"></param>
    /// <param name="flags"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> ObjectGetAsync<T>(
        this IDatabase redis,
        string key,
        CommandFlags flags = CommandFlags.None
    )
        where T : class?
    {
        string? stringResult = await redis.StringGetAsync(key, flags);
        if (stringResult == null)
            return null;
        return JsonSerializer.Deserialize<T>(stringResult);
    }

    /// <summary>
    /// 将对象json解析后放进缓存
    /// 请手动解决循环引用问题！
    /// </summary>
    /// <param name="redis"></param>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    /// <param name="expiry"></param>
    /// <param name="keepTtl"></param>
    /// <param name="when"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static async Task<bool> ObjectSetAsync(
        this IDatabase redis,
        string key,
        object obj,
        TimeSpan? expiry = null,
        bool keepTtl = false,
        When when = When.Always,
        CommandFlags flags = CommandFlags.None
    )
    {
        string value = JsonSerializer.Serialize(obj, IgnoreWriteDefaultOption);
        return await redis.StringSetAsync(key, value, expiry, keepTtl, when, flags);
    }

    public static async Task<RedisLock> LockAsync(
        this IDatabase redis,
        RedisKey key,
        TimeSpan expiry
    )
    {
        return await RedisLock.Lock(redis, key, expiry);
    }

    public static async Task<RedisLock> LockAsync(
        this IDatabase redis,
        RedisKey key,
        RedisValue value,
        TimeSpan expiry
    )
    {
        return await RedisLock.Lock(redis, key, value, expiry);
    }
}
