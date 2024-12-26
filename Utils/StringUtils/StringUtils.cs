namespace Utils.StringUtils;

public class StringUtils
{
    private static readonly char[] RandomChars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    public static string GenerateRandomString(int length)
    {
        Random random = new Random();
        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = RandomChars[random.Next(0, RandomChars.Length)];
        return new string(chars);
    }

    public static string GenerateRandomString(int length, int seed)
    {
        Random random = new Random(seed);
        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
            chars[i] = RandomChars[random.Next(0, RandomChars.Length)];
        return new string(chars);
    }
}
