using System;

namespace OtakuGifs.Exceptions;

public class OtakuGifsException : Exception
{
    public OtakuGifsException(string message) : base(message)
    {
    }

    public OtakuGifsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}