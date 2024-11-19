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

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberToken> MemberTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Memcode).HasComment("用户唯一标识");
            entity.Property(e => e.Nickname).HasComment("昵称");
            entity.Property(e => e.Password).HasComment("密码");
            entity.Property(e => e.UpdateTime).ValueGeneratedOnAddOrUpdate();
            entity.Property(e => e.Username).HasComment("用户名");
        });

        modelBuilder.Entity<MemberToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.LastLoginTime).HasComment("最后登录时间");
            entity.Property(e => e.MemberId).HasComment("用户id");
            entity.Property(e => e.Token).HasComment("token");
            entity.Property(e => e.UpdateTime).ValueGeneratedOnAddOrUpdate();

            entity.HasOne(d => d.Member).WithMany(p => p.MemberTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("member_token_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
