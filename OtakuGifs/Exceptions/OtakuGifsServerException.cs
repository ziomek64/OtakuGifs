using System;

namespace OtakuGifs.Exceptions;

public class OtakuGifsServerException : OtakuGifsException
{
    public int StatusCode { get; }

    public OtakuGifsServerException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}