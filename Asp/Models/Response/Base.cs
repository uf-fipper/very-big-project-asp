using System.Text.Json.Serialization;

namespace Asp.DataModels;

public class Result(int ret, string message)
{
    public int Ret { get; set; } = ret;

    public string Message { get; set; } = message;

    public static ResultSuccess Success() => new();

    public static ResultSuccess Success(string message) => new(message);

    public static ResultSuccess<T> Success<T>(T data) => new(data);

    public static ResultSuccess<T> Success<T>(T data, string message) => new(data, message);

    public static ResultError Error() => new();

    public static ResultError Error(string message) => new(message: message);

    public static ResultError<T> Error<T>(T data) => new(data);

    public static ResultError<T> Error<T>(T data, string message) => new(data, message: message);

    public static ResultError ErrorWithCode(string code) => new(code);

    public static ResultError ErrorWithCode(string code, string message) => new(code, message);

    public static ResultError<T> ErrorWithCode<T>(T data, string code) => new(data, code);

    public static ResultError<T> ErrorWithCode<T>(T data, string code, string message) =>
        new(data, code, message);
}

public class ResultSuccess(string message = "success") : Result(1, message) { }

public class ResultSuccess<T>(T data, string message = "success") : ResultSuccess(message)
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = data;
}

public class ResultError(string code = "error", string message = "error") : Result(0, message)
{
    public string Code { get; set; } = code;
}

public class ResultError<T>(T data, string code = "error", string message = "error")
    : ResultError(code, message)
{
    [JsonPropertyName("data")]
    public T Data { get; set; } = data;
}
