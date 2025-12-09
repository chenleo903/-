namespace CrmSystem.Api.Exceptions;

/// <summary>
/// 冲突异常（唯一性约束等）
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
