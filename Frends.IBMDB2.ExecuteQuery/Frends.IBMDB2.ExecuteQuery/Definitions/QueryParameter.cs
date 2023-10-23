namespace Frends.IBMDB2.ExecuteQuery.Definitions;

/// <summary>
/// Query parameters for the query.
/// </summary>
public class QueryParameter
{
    /// <summary>
    /// Name of the parameter.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Value for the parameter.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Data type of the parameter.
    /// </summary>
    public DataTypes DataType { get; set; }
}