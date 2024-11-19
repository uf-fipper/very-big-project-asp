using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Models.Models;

namespace Asp.Models.Responses.Members;

public class ResMember
{
    public string? Memcode { get; set; }

    public required string Username { get; set; }

    public required string Nickname { get; set; }

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
