namespace Frends.IBMDB2.ExecuteQuery.Definitions;

using System;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Login failed for user 'user'.</example>
    public string Message { get; set; }

    /// <summary>
    /// The original exception that caused the error.
    /// </summary>
    /// <example>object { Exception AdditionalInfo }</example>
    public Exception AdditionalInfo { get; set; }
}
