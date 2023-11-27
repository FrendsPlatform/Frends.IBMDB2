namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// Specifies how a command string is interpreted.
/// </summary>
public enum ExecuteType
{
    /// <summary>
    /// ExecuteReader for SELECT-query and NonQuery for UPDATE, INSERT, or DELETE statements.
    /// </summary>
    Auto,

    /// <summary>
    /// Use this operation to execute any arbitrary SQL statements in SQL Server if you do not want any result set to be returned.
    /// You can use this operation to create database objects or change data in a database by executing UPDATE, INSERT, or DELETE statements.
    /// The return value of this operation is of Int32 data type, and For the UPDATE, INSERT, and DELETE statements, the return
    /// value is the number of rows affected by the SQL statement. For all other types of statements, the return value is -1.
    /// </summary>
    NonQuery,

    /// <summary>
    /// Use this operation to execute any arbitrary SQL statements in SQL Server to return a single value.
    /// This operation returns the value only in the first column of the first row in the result set returned by the SQL statement.
    /// </summary>
    Scalar,

    /// <summary>
    /// Use this operation to execute any arbitrary SQL statements in SQL Server if you want the result set to be returned.
    /// </summary>
    ExecuteReader,
}