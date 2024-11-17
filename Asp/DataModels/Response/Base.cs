using System.Text.Json.Serialization;

namespace Asp.DataModels;

public class Result(int ret, string message = "")
{
    [JsonPropertyName("ret")] public int Ret { get; set; } = ret;
    [JsonPropertyName("message")] public string Message { get; set; } = message;

    public static ResultSuccess Success() => new();
    public static ResultSuccess Success(string message) => new(message);

    public static ResultSuccess<T> Success<T>(T data) => new(data);
    public static ResultSuccess<T> Success<T>(T data, string message) => new(data, message);

    public static ResultError Error() => new();
    public static ResultError Error(string message) => new(message);

    public static ResultError<T> Error<T>(T data) => new(data);
    public static ResultError<T> Error<T>(T data, string message) => new(data, message);
}

public class ResultSuccess(string message = "success") : Result(1, message) { }

public class ResultSuccess<T>(T data, string message = "success") : ResultSuccess(message)
{
    [JsonPropertyName("data")] public T Data { get; set; } = data;
}

public class ResultError(string message = "error") : Result(0, message) { }

public class ResultError<T>(T data, string message = "error") : ResultError(message)
{
    [JsonPropertyName("data")] public T Data { get; set; } = data;
}