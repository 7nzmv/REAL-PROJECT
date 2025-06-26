using System.Net;

namespace Domain.Responses;

public class Response<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }

    public Response()
    {

    }

    public Response(T? data)
    {
        Data = data;
        IsSuccess = true;
        StatusCode = 200;
        Message = null;
    }

    public Response(HttpStatusCode statusCode, string message)
    {
        IsSuccess = false;
        Data = default;
        StatusCode = (int)statusCode;
        Message = message;
    }
}



