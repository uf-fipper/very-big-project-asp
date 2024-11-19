using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Asp.Models.Requests.Members;

public class ReqRegister
{
    [Required(ErrorMessage = "请输入用户名")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "用户名必须在6到20个字符之间")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "请输入密码")]
    [StringLength(32, MinimumLength = 6, ErrorMessage = "密码必须在6到20个字符之间")]
    public required string Password { get; set; }
}
