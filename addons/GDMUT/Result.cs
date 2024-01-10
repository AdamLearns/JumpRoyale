#if TOOLS
using System;

namespace GdMUT;

/// <summary>
/// An objecting denoting the result of an operation.
/// </summary>
public struct Result
{
    /// <summary>
    /// A successful result.
    /// </summary>
    public static readonly Result Success = new(true, string.Empty);

    /// <summary>
    /// A failed result.
    /// </summary>
    public static readonly Result Failure = new(false, string.Empty);

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="success">Whether the operation was a success.</param>
    /// <param name="message">Custom message.</param>
    public Result(bool success, string message = "")
    {
        IsSuccess = success;
        Message = message;
    }

    /// <summary>
    /// Whether the operation was a success or not.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// The message of the result.
    /// </summary>
    public string Message { get; set; }
}
#endif
