﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Models.Context;

#nullable disable

namespace Models.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20241121150238_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Models.Models.Member", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("Memcode")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)")
                        .HasColumnName("memcode")
                        .HasComment("用户唯一标识");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("nickname")
                        .HasComment("昵称");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("password")
                        .HasComment("密码");

                    b.Property<DateTime?>("UpdateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("update_time");

                    MySqlPropertyBuilderExtensions.UseMySqlComputedColumn(b.Property<DateTime?>("UpdateTime"));

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varchar(64)")
                        .HasColumnName("username")
                        .HasComment("用户名");

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "Memcode" }, "memcode")
                        .IsUnique();

                    b.HasIndex(new[] { "Username" }, "username")
                        .IsUnique();

                    b.HasIndex(new[] { "Username", "Password" }, "username_password");

                    b.ToTable("member");
                });

            modelBuilder.Entity("Models.Models.MemberToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("create_time")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("LastLoginTime")
                        .HasColumnType("datetime")
                        .HasColumnName("last_login_time")
                        .HasComment("最后登录时间");

                    b.Property<int>("MemberId")
                        .HasColumnType("int")
                        .HasColumnName("member_id")
                        .HasComment("用户id");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("varchar(128)")
                        .HasColumnName("token")
                        .HasComment("token");

                    b.Property<DateTime?>("UpdateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime")
                        .HasColumnName("update_time");

                    MySqlPropertyBuilderExtensions.UseMySqlComputedColumn(b.Property<DateTime?>("UpdateTime"));

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "MemberId", "Token" }, "member_id")
                        .IsUnique();

                    b.HasIndex(new[] { "Token" }, "token")
                        .IsUnique();

                    b.ToTable("member_token");
                });

            modelBuilder.Entity("Models.Models.MemberToken", b =>
                {
                    b.HasOne("Models.Models.Member", "Member")
                        .WithMany("MemberTokens")
                        .HasForeignKey("MemberId")
                        .IsRequired()
                        .HasConstraintName("member_token_ibfk_1");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("Models.Models.Member", b =>
                {
                    b.Navigation("MemberTokens");
                });
#pragma warning restore 612, 618
        }
    }
}