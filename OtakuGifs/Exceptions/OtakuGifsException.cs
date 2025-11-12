using System;

namespace OtakuGifs.Exceptions;

/// <summary>
/// Base exception for all OtakuGifs library errors
/// </summary>
public class OtakuGifsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the OtakuGifsException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public OtakuGifsException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OtakuGifsException class with a specified error message and inner exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public OtakuGifsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}