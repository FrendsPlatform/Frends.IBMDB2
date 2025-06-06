﻿namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// Result class usually contains properties of the return object.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete without errors.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; init; }

    /// <summary>
    /// Records affected.
    /// Some statements will return -1. See documentation of Input.ExecuteType for more information.
    /// </summary>
    /// <example>100</example>
    public int RecordsAffected { get; init; } = 0;

    /// <summary>
    /// Error message.
    /// This value is generated when an exception occurs and Options.ThrowErrorOnFailure = false.
    /// </summary>
    /// <example>Login failed for user 'user'.</example>
    public string ErrorMessage { get; init; } = null;

    /// <summary>
    /// Query result as JToken.
    /// </summary>
    /// <example>
    /// Input.ExecuteType = ExecuteReader: [{"ID": "1","FIRST_NAME": "Saija","LAST_NAME": "Saijalainen","START_DATE": ""}],
    /// Input.ExecuteType = NonQuery: {{  "AffectedRows": -1 }},
    /// Input.ExecuteType = Scalar: {{  "Value": 1 }}
    /// </example>
    public dynamic Data { get; init; } = null;
}
