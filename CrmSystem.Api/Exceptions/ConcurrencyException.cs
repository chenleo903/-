namespace CrmSystem.Api.Exceptions;

/// <summary>
/// 并发冲突异常
/// </summary>
public class ConcurrencyException : Exception
{
    public DateTimeOffset CurrentVersion { get; }

    public ConcurrencyException(string message, DateTimeOffset currentVersion)
        : base(message)
    {
        CurrentVersion = currentVersion;
    }
}
