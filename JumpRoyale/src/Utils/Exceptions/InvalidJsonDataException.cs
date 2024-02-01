using System;

public class InvalidJsonDataException : Exception
{
    public InvalidJsonDataException()
        : base(JsonDeserializationConstants.JsonDeserializationError) { }

    public InvalidJsonDataException(string message)
        : base(message) { }

    public InvalidJsonDataException(string message, Exception innerException)
        : base(message, innerException) { }
}
