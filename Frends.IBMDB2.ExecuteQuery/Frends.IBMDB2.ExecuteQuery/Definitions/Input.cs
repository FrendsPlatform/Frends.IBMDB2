namespace Frends.IBMDB2.ExecuteQuery.Definitions;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Connection string to the IBMDB2 database.
    /// </summary>
    /// <example>Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;</example>
    [DisplayFormat(DataFormatString = "Text")]
    [DefaultValue("Server = myAddress:myPortNumber;Database = myDataBase;UID = myUsername;PWD = myPassword;")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Query to be executed in string format.
    /// </summary>
    /// <example>SELECT * FROM MyTable</example>
    [DisplayFormat(DataFormatString = "Sql")]
    [DefaultValue("")]
    public string Query { get; set; }

    /// <summary>
    /// Parameters for the database query.
    /// </summary>
    /// <example>[ { Name = "id", Value = 1, DataType = SqlDataTypes.Auto }, { Name = "first_name", Value = "John", DataType = SqlDataTypes.NVarChar }, { Name = "last_name", Value = "Doe", DataType = SqlDataTypes.NVarChar } ]</example>
    public QueryParameter[] Parameters { get; set; }

    /// <summary>
    /// Specifies how a command string is interpreted.
    /// </summary>
    /// <example>ExecuteType.ExecuteReader</example>
    public ExecuteType ExecuteType { get; set; }
}