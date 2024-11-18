using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Models;

[Table("member_token")]
[Index("MemberId", "Token", Name = "member_id", IsUnique = true)]
[Index("Token", Name = "token", IsUnique = true)]
public partial class MemberToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    [Column("member_id")]
    public int MemberId { get; set; }

    /// <summary>
    /// token
    /// </summary>
    [Column("token")]
    [StringLength(128)]
    public string Token { get; set; } = null!;

    /// <summary>
    /// 状态
    /// </summary>
    [Column("status")]
    public int Status { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [Column("last_login_time", TypeName = "datetime")]
    public DateTime LastLoginTime { get; set; }

    /// <summary>
    /// 最后使用时间
    /// </summary>
    [Column("last_use_time", TypeName = "datetime")]
    public DateTime LastUseTime { get; set; }

    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Column("update_time", TypeName = "datetime")]
    public DateTime? UpdateTime { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("MemberTokens")]
    public virtual Member Member { get; set; } = null!;
}
