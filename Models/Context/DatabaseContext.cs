using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Models.Context;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatPrivateMessage> ChatPrivateMessages { get; set; }

    public virtual DbSet<EfmigrationsHistory> EfmigrationsHistories { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberToken> MemberTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<ChatPrivateMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chat_private_message", tb => tb.HasComment("私聊消息表"));

            entity.Property(e => e.Content).HasComment("消息详情");
            entity.Property(e => e.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.MemberId).HasComment("发送者");
            entity.Property(e => e.Seq).HasComment("消息序列号");
            entity.Property(e => e.ToMemberId).HasComment("接收者");
            entity.Property(e => e.UpdateTime).ValueGeneratedOnAddOrUpdate();
            entity.Property(e => e.Version)
                .HasDefaultValueSql("'1'")
                .HasComment("消息详情版本号");

            entity.HasOne(d => d.Member).WithMany(p => p.ChatPrivateMessageMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chat_private_message_ibfk_1");

            entity.HasOne(d => d.ToMember).WithMany(p => p.ChatPrivateMessageToMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chat_private_message_ibfk_2");
        });

        modelBuilder.Entity<EfmigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Memcode).HasComment("用户唯一标识");
            entity.Property(e => e.Nickname).HasComment("昵称");
            entity.Property(e => e.Password).HasComment("密码");
            entity.Property(e => e.UpdateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Username).HasComment("用户名");
        });

        modelBuilder.Entity<MemberToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.LastLoginTime).HasComment("最后登录时间");
            entity.Property(e => e.MemberId).HasComment("用户id");
            entity.Property(e => e.Token).HasComment("token");
            entity.Property(e => e.UpdateTime)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("member_token_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
