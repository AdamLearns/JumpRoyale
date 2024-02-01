using System;

public class MissingStatsFilePathException : Exception
{
    public MissingStatsFilePathException()
        : base(PlayerStatsMessages.MissingPlayerStatsPath) { }

    public MissingStatsFilePathException(string message)
        : base(message) { }

    public MissingStatsFilePathException(string message, Exception innerException)
        : base(message, innerException) { }
}
