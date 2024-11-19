namespace Asp.Models.Responses.Members;

public class ResLoginMember : ResMember
{
    public string Token { get; set; } = null!;

    public bool IsUpdate { get; set; }
}
