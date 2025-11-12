using System;

namespace OtakuGifs.Exceptions;

/// <summary>
/// Exception thrown when the API returns a 4xx client error status code
/// </summary>
public class OtakuGifsBadRequestException : OtakuGifsException
{
    /// <summary>
    /// Gets the HTTP status code returned by the API
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the OtakuGifsBadRequestException class with a specified error message and status code
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="statusCode">The HTTP status code returned by the API</param>
    public OtakuGifsBadRequestException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}