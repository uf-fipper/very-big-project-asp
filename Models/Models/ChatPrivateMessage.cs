using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Models.Models;

/// <summary>
/// 私聊消息表
/// </summary>
[Table("chat_private_message")]
[Index("MemberId", Name = "chat_private_message_ibfk_1")]
[Index("ToMemberId", Name = "to_member_id")]
public partial class ChatPrivateMessage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// 发送者
    /// </summary>
    [Column("member_id")]
    public int MemberId { get; set; }

    /// <summary>
    /// 接收者
    /// </summary>
    [Column("to_member_id")]
    public int ToMemberId { get; set; }

    /// <summary>
    /// 消息详情版本号
    /// </summary>
    [Column("version")]
    public int Version { get; set; }

    /// <summary>
    /// 消息详情
    /// </summary>
    [Column("content", TypeName = "text")]
    public string Content { get; set; } = null!;

    /// <summary>
    /// 消息序列号
    /// </summary>
    [Column("seq")]
    public int Seq { get; set; }

    [Column("create_time", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Column("update_time", TypeName = "datetime")]
    public DateTime? UpdateTime { get; set; }

    [ForeignKey("MemberId")]
    [InverseProperty("ChatPrivateMessageMembers")]
    public virtual Member Member { get; set; } = null!;

    [ForeignKey("ToMemberId")]
    [InverseProperty("ChatPrivateMessageToMembers")]
    public virtual Member ToMember { get; set; } = null!;
}
