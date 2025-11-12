using System;

namespace OtakuGifs.Exceptions;

/// <summary>
/// Exception thrown when the API returns a 5xx server error status code
/// </summary>
public class OtakuGifsServerException : OtakuGifsException
{
    /// <summary>
    /// Gets the HTTP status code returned by the API
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the OtakuGifsServerException class with a specified error message and status code
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="statusCode">The HTTP status code returned by the API</param>
    public OtakuGifsServerException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}