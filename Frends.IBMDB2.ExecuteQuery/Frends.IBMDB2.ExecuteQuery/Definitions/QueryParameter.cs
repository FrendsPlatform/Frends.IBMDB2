namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// Query parameters for the query.
/// </summary>
public class QueryParameter
{
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    /// <example>id</example>
    public string Name { get; set; }

    /// <summary>
    /// Value for the parameter.
    /// </summary>
    /// <example>1</example>
    public string Value { get; set; }

    /// <summary>
    /// Data type of the parameter.
    /// </summary>
    /// <example>DataTypes.Int</example>
    public DataTypes DataType { get; set; }
}