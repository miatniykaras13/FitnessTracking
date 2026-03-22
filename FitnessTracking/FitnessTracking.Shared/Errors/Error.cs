using System.Text.Json.Serialization;

namespace FitnessTracking.Shared.Errors;

public class Error
{
    public string Code { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; init; }

    public string? Message { get; init; }

    [JsonConstructor]
    private Error(string code, ErrorType type, string? message = null)
    {
        Code = code;
        Type = type;
        Message = message;
    }

    /// <summary>
    /// Represents validation error
    /// </summary>
    /// <param name="obj">invalid object.</param>
    /// <param name="message">additional info.</param>
    /// <returns>Error.</returns>
    public static Error Validation(string obj, string? message = null) =>
        new($"{obj}.is_invalid", ErrorType.Validation, message);


    /// <summary>
    /// Represents not found error
    /// </summary>
    /// <param name="obj">not found obj.</param>
    /// <param name="message">additional info.</param>
    /// <returns>Error.</returns>
    public static Error NotFound(string obj, string? message = null) =>
        new($"{obj}.not_found", ErrorType.NotFound, message);

    /// <summary>
    /// Represents internal server error
    /// </summary>
    /// <param name="code">error code.</param>
    /// <param name="message">additional info.</param>
    /// <returns>Error.</returns>
    public static Error Internal(string? code = null, string? message = null) =>
        new(code ?? $"internal_error", ErrorType.Internal, message);

    /// <summary>
    /// Represents conflict error
    /// </summary>
    /// <param name="obj">conflicting obj.</param>
    /// <param name="message">additional info.</param>
    /// <returns>Error.</returns>
    public static Error Conflict(string obj, string? message = null) =>
        new($"{obj}.is_conflict", ErrorType.Conflict, message);

    /// <summary>
    /// Represents unknown error
    /// </summary>
    /// <param name="message">additional info.</param>
    /// <returns>Error.</returns>
    public static Error Unknown(string? message = null) =>
        new($"unknown_error", ErrorType.Unknown, message);

    /// <summary>
    /// Represents domain error. Error code should be fully entered
    /// </summary>
    /// <param name="code">error code.</param>
    /// <param name="message">additional info.</param>
    /// <returns>Error.</returns>
    public static Error Domain(string code, string? message = null) =>
        new(code, ErrorType.Domain, message);

    public static Error Forbidden(string obj, string? message = null) =>
        new($"{obj}.is_forbidden", ErrorType.Forbidden, message);

    public static Error Custom(string code, string? message = null, ErrorType? errorType = null) =>
        new(code, errorType ?? ErrorType.Unknown, message);

    public static implicit operator List<Error>(Error error) => [error];
}