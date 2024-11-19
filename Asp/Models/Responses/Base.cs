using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Asp.Models.Responses;

public abstract class Result(int ret)
{
    public int Ret { get; set; } = ret;

    public long Timestamp { get; set; } = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();

    public static ResultSuccess<T> Success<T>(T data) => new(data);

    public static ResultError<T> Error<T>(T data) => new(data);

    public static ResultError<T> Error<T>(T data, string code) => new(data, code);

    public static ResultError<string> TokenExpired() => new("请登录", "401");

    public static ResultError<T> TokenExpired<T>(T data) => new(data, "401");
}

public abstract class ResultSuccess() : Result(1) { }

public class ResultSuccess<T>(T data) : ResultSuccess
{
    public T Data { get; set; } = data;
}

public abstract class ResultError() : Result(0)
{
    public abstract string Code { get; set; }
}

public class ResultError<T>(T data, string? code = null) : ResultError
{
    public T Data { get; set; } = data;

    public override string Code { get; set; } = code ?? "error";
}
