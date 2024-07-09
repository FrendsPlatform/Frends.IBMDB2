namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// IBM DB2-specific data type.
/// </summary>
public enum DataTypes
{
#pragma warning disable CS1591 // self explanatory
    Auto = -1,
    BigInt = 0,
    Binary = 1,
    Char = 3,
    DateTime = 4,
    Decimal = 5,
    Float = 6,
    Integer = 8,
    Money = 9,
    NChar = 10,
    NVarChar = 12,
    Real = 13,
    SmallInt = 16,
    Text = 18,
    Timestamp = 19,
    VarBinary = 21,
    VarChar = 22,
    Xml = 25,
    Date = 31,
    Time = 32,
#pragma warning restore CS1591 // self explanatory
}