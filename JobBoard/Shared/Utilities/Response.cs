using System.Text.Json.Serialization;

namespace JobBoard.Shared.Utilities;

public record Error(ErrorTypes ErrorType, string ErrorMessage);
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ErrorTypes
{
    NotFound,
    InvalidRole,
    Conflict,
    Internal,
    Unauthorized
}


/// <summary>
/// A class represent result pattern of type <c><typeparamref name="T"/></c>
/// </summary>
/// 
public class Response<T>

{
    /// <summary>
    /// Indicates wheter the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }
    /// <summary>
    /// The data returned if operation was successful;otherwise, null.
    /// </summary>
    public T? Data { get; set; }
    /// <summary>
    /// A list of error messages if the operation failed;eempty if successful.
    /// </summary>
    public List<Error> Errors { get; set; } = [];

    /// <summary>
    /// Private constructor for full response initialization.
    /// </summary>
    /// <param name="data">The data payload./>.</param>
    /// <param name="isSuccess">The success status of the operation.</param>
    /// <param name="errors">A list of errors if any occurred.</param>
    private Response(T? data, bool isSuccess, List<Error>? errors)
    {

        IsSuccess = isSuccess;
        Data = data;
        Errors = errors ?? Errors;
    }
    /// <summary>
    /// Private constructor for response initialization without data.
    /// </summary>
    /// <param name="isSuccess">The success status of the operation.</param>
    /// <param name="errors">A list of errors if any occurred.</param>
    private Response(bool isSuccess, List<Error>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Errors;
    }

    /// <summary>
    /// Creates a successful response without any data.
    /// </summary>
    /// <returns>A successful <c>Response</c> instance of type <c><c><typeparamref name="T"/></c></c>.</returns>
    public static Response<T> Success() => new(true, null);

    /// <summary>
    /// Creates a successful response witany data.
    /// </summary>
    /// <returns>A successful <c>Response</c> instance of type <c><typeparamref name="T"/></c>.</returns>
    public static Response<T> Success(T data) => new(data, true, null);
    /// <summary>
    /// Creates a failed response witout any data.
    /// </summary>
    /// <returns>a failed operation with error messages in <c>Response.Errors</c>.</returns>
    public static Response<T> Failure(List<Error> errors) => new(false, errors);
    /// <summary>
    /// Creates a failed response witout any data.
    /// </summary>
    /// <returns>a failed operation with error messages in <c>Response.Errors</c>.</returns>
    public static Response<T> Failure(Error error) => new(false, [error]);

}
