using System;

namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Login failed for user 'user'.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>Exception</example>
    public Exception AdditionalInfo { get; set; }
}
