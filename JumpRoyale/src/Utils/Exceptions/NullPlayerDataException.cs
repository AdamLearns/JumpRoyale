using System;

public class NullPlayerDataException : Exception
{
    public NullPlayerDataException()
        : base(PlayerStatsMessages.NullPlayerData) { }

    public NullPlayerDataException(string message)
        : base(message) { }

    public NullPlayerDataException(string message, Exception innerException)
        : base(message, innerException) { }
}
