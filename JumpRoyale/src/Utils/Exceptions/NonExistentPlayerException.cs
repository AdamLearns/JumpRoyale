using System;

public class NonExistentPlayerException : Exception
{
    public NonExistentPlayerException()
        : base(PlayerStatsMessages.NonExistentPlayer) { }

    public NonExistentPlayerException(string message)
        : base(message) { }

    public NonExistentPlayerException(string message, Exception innerException)
        : base(message, innerException) { }
}
