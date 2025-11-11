using System;

namespace OtakuGifs.Exceptions;

public class OtakuGifsBadRequestException : OtakuGifsException
{
    public int StatusCode { get; }

    public OtakuGifsBadRequestException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}