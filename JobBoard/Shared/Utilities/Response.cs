namespace JobBoard.Shared.Utilities;

public class Response<T>

{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];

    private Response(T? data, bool isSuccess, List<string>? errors)
    {
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors ?? Errors;
    }

    private Response(bool isSuccess, List<string>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors ?? Errors;
    }

    public static Response<T> Success() => new(true, null);
    public static Response<T> Success(T data) => new(data, true, null);
    public static Response<T> Failure(List<string> errors) => new(false, errors);
    public static Response<T> Failure(string error) => new(false, [error]);

}
