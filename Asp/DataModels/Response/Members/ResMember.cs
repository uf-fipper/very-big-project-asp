using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Models;

namespace Asp.DataModels.Members;

public class ResMember
{
    [JsonPropertyName("memcode")]
    public string? Memcode { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("nickname")]
    public required string Nickname { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [return: NotNullIfNotNull(nameof(member))]
    public static implicit operator ResMember?(Member? member) => FromMember(member);

    [return: NotNullIfNotNull(nameof(member))]
    public static ResMember? FromMember(Member? member) =>
        member == null
            ? null
            : new ResMember
            {
                Memcode = member.Memcode,
                Username = member.Username,
                Nickname = member.Nickname,
            };
}
