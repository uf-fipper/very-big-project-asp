using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models.ModelsExt;

public static partial class MemberExt { }

public interface IChatSerialize { }

public class MyJsonDerivedTypeAttribute<T> : JsonDerivedTypeAttribute
    where T : ChatMessageSegment
{
    private static string GetChatMessageSegmentType()
    {
        Type type = typeof(T);
        var typeProperty = type.GetProperty(
            "Type",
            BindingFlags.Static | BindingFlags.Public,
            null,
            typeof(string),
            [],
            null
        )!;
        string? typeText = typeProperty.GetValue(null) as string;
        if (string.IsNullOrEmpty(typeText))
            throw new ArgumentNullException("Type must not be null.");
        return typeText;
    }

    public MyJsonDerivedTypeAttribute()
        : base(typeof(T), GetChatMessageSegmentType()) { }
}

public class ChatMessageException(string message) : Exception(message);

public class ChatSerializeException(string message) : ChatMessageException(message);

public class ChatDeSerializeException(string message) : ChatMessageException(message);

public class ChatMessage : List<ChatMessageSegment>
{
    public ChatMessage()
        : base() { }

    public ChatMessage(int capacity)
        : base(capacity) { }

    public ChatMessage(IEnumerable<ChatMessageSegment> messageSegments)
        : base(messageSegments) { }

    public ChatMessage(ChatMessageSegment segment)
        : base([segment]) { }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static ChatMessage Deserialize(string message)
    {
        return JsonSerializer.Deserialize<ChatMessage>(message)!;
    }
}

[MyJsonDerivedTypeAttribute<ChatMessageSegmentText>]
[MyJsonDerivedTypeAttribute<ChatMessageSegmentImage>]
[MyJsonDerivedTypeAttribute<ChatMessageSegmentEmoji>]
[MyJsonDerivedTypeAttribute<ChatMessageSegmentUnknown>]
public abstract class ChatMessageSegment : IChatSerialize
{
    public static string Type { get; } = "unknown";

    public static ChatMessageSegmentText Text(string text) => new() { Content = text };

    public static ChatMessageSegmentImage Image(string url) => new() { Url = url };

    public static ChatMessageSegmentEmoji Emoji(string id) => new() { EmojiId = id };
}

public class ChatMessageSegmentText : ChatMessageSegment
{
    public string Content { get; set; } = null!;

    public static implicit operator ChatMessageSegmentText(string text) => new() { Content = text };
}

public class ChatMessageSegmentImage : ChatMessageSegment
{
    public string Url { get; set; } = null!;
}

public class ChatMessageSegmentEmoji : ChatMessageSegment
{
    public string EmojiId { get; set; } = null!;
}

public class ChatMessageSegmentUnknown : ChatMessageSegment
{
    public Dictionary<string, object> Data { get; set; } = [];
}
