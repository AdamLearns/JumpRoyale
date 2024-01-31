using System;

public class DuplicatePlayerException : Exception
{
    public DuplicatePlayerException()
        : base(PlayerStatsMessages.DuplicatePlayerStoreError) { }

    public DuplicatePlayerException(string message)
        : base(message) { }

    public DuplicatePlayerException(string message, Exception innerException)
        : base(message, innerException) { }
}
