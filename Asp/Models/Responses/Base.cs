using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Asp.Models.Responses;

public class Result(int ret, long? timestamp = null)
{
    public int Ret { get; set; } = ret;

    public long Timestamp { get; set; } = timestamp ?? DateTime.Now.Millisecond;

    public static ResultSuccess<T> Success<T>(T data) => new(data);

    public static ResultError<T> Error<T>(T data) => new(data);

    public static ResultError<T> Error<T>(T data, string code) => new(data, code);
}

public class ResultSuccess<T>(T data) : Result(1)
{
    public T Data { get; set; } = data;
}

public class ResultError<T>(T data, string? code = null) : Result(0)
{
    public T Data { get; set; } = data;

    public string Code { get; set; } = code ?? "error";
}
