using StackExchange.Redis;

namespace Asp.Utils.RedisExt;

public class RedisLock : IAsyncDisposable
{
    public IDatabase Redis { get; }
    public RedisKey Key { get; }
    private readonly RedisValue _lockValue;
    public bool Locked { get; private set; }

    private RedisLock(IDatabase redis, RedisKey key, RedisValue? lockValue = null)
    {
        Redis = redis;
        Key = key;
        _lockValue = lockValue ?? Guid.NewGuid().ToString();
    }

    public static async Task<RedisLock> Lock(IDatabase redis, RedisKey key, TimeSpan expiry)
    {
        var redisLock = new RedisLock(redis, key);
        redisLock.Locked = await redis.LockTakeAsync(key, redisLock._lockValue, expiry);
        return redisLock;
    }

    public static async Task<RedisLock> Lock(
        IDatabase redis,
        RedisKey key,
        RedisValue value,
        TimeSpan expiry
    )
    {
        var redisLock = new RedisLock(redis, key, value);
        redisLock.Locked = await redis.LockTakeAsync(key, redisLock._lockValue, expiry);
        return redisLock;
    }

    public async ValueTask DisposeAsync()
    {
        await Redis.LockReleaseAsync(Key, _lockValue);
    }
}
