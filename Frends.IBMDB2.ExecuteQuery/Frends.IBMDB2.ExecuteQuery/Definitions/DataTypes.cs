namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// IBM DB2-specific data type.
/// </summary>
public enum DataTypes
{
    /// <summary>
    /// Auto data type.
    /// </summary>
    Auto = -1,

    /// <summary>
    /// BigInt data type.
    /// </summary>
    BigInt = 0,

    /// <summary>
    /// Binary data type.
    /// </summary>
    Binary = 1,

    /// <summary>
    /// Char data type.
    /// </summary>
    Char = 3,

    /// <summary>
    /// DateTime data type.
    /// </summary>
    DateTime = 4,

    /// <summary>
    /// Decimal data type.
    /// </summary>
    Decimal = 5,

    /// <summary>
    /// Float data type.
    /// </summary>
    Float = 6,

    /// <summary>
    /// Integer data type.
    /// </summary>
    Integer = 8,

    /// <summary>
    /// Money data type.
    /// </summary>
    Money = 9,

    /// <summary>
    /// NChar data type.
    /// </summary>
    NChar = 10,

    /// <summary>
    /// NVarChar data type.
    /// </summary>
    NVarChar = 12,

    /// <summary>
    /// Real data type.
    /// </summary>
    Real = 13,

    /// <summary>
    /// SmallInt data type.
    /// </summary>
    SmallInt = 16,

    /// <summary>
    /// Text data type.
    /// </summary>
    Text = 18,

    /// <summary>
    /// Timestamp data type.
    /// </summary>
    Timestamp = 19,

    /// <summary>
    /// VarBinary data type.
    /// </summary>
    VarBinary = 21,

    /// <summary>
    /// VarChar data type.
    /// </summary>
    VarChar = 22,

    /// <summary>
    /// Xml data type.
    /// </summary>
    Xml = 25,

    /// <summary>
    /// Date data type.
    /// </summary>
    Date = 31,

    /// <summary>
    /// Time data type.
    /// </summary>
    Time = 32,
}