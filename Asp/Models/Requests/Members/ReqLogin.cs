using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Asp.Models.Requests.Members;

public class ReqLogin
{
    [Required]
    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [Required]
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}
