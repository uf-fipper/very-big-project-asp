using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Models;

[Table("member")]
[Index("Memcode", Name = "memcode", IsUnique = true)]
[Index("Username", Name = "username", IsUnique = true)]
[Index("Username", "Password", Name = "username_password")]
public partial class Member
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 用户唯一标识
    /// </summary>
    [Column("memcode")]
    [StringLength(64)]
    public string Memcode { get; set; } = null!;

    /// <summary>
    /// 用户名
    /// </summary>
    [Column("username")]
    [StringLength(64)]
    public string Username { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    [Column("password")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 昵称
    /// </summary>
    [Column("nickname")]
    [StringLength(255)]
    public string Nickname { get; set; } = null!;

    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Column("update_time", TypeName = "datetime")]
    public DateTime? UpdateTime { get; set; }

    [InverseProperty("Member")]
    public virtual ICollection<MemberToken> MemberTokens { get; set; } = new List<MemberToken>();
}
