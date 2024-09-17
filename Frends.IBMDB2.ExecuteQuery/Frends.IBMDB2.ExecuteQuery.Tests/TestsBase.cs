#pragma warning disable SA1600 // Elements should be documented
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Frends.IBMDB2.ExecuteQuery;
using Frends.IBMDB2.ExecuteQuery.Definitions;
using IBM.Data.Db2;
using NUnit.Framework;

internal abstract class TestsBase
{
    protected static readonly string ConnString =
        "Server=localhost:50000;Database=testdb;User Id=db2inst1;Password=password";

    protected static readonly string TableName = "TestTable";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        TestConnectionBeforeRunningTests(ConnString);
    }

    [SetUp]
    public async Task Init()
    {
        var input = new Input()
        {
            ConnectionString = ConnString,
            Query =
                $@"CREATE TABLE db2inst1.{TableName} ( Id int, LASTNAME varchar(255), FIRSTNAME varchar(255) );",
            ExecuteType = ExecuteType.NonQuery,
            Parameters = null,
        };
        await IBMDB2.ExecuteQuery(input, new Options(), default);
    }

    [TearDown]
    public async Task CleanUp()
    {
        var input = new Input()
        {
            ConnectionString = ConnString,
            Query = $@"DROP TABLE IF EXISTS db2inst1.{TableName};",
            ExecuteType = ExecuteType.NonQuery,
            Parameters = null,
        };
        await IBMDB2.ExecuteQuery(input, new Options(), default);
    }

    protected static List<QueryParameter> TestParameters() =>
        new()
        {
            new()
            {
                Name = "id",
                Value = "1",
                DataType = DataTypes.Auto,
            },
        };

    protected static Options TestOptions() =>
        new()
        {
            SqlTransactionIsolationLevel = TransactionIsolationLevel.ReadCommitted,
            ConnectionTimeoutSeconds = 2,
            ThrowErrorOnFailure = true,
        };

    protected static async Task<int> GetRowCount()
    {
        var input = new Input()
        {
            ConnectionString = ConnString,
            Query = $"SELECT COUNT(*) FROM {TableName};",
            ExecuteType = ExecuteType.Scalar,
            Parameters = null,
        };

        var options = new Options()
        {
            SqlTransactionIsolationLevel = TransactionIsolationLevel.None,
            ConnectionTimeoutSeconds = 2,
            ThrowErrorOnFailure = true,
        };

        var result = await IBMDB2.ExecuteQuery(input, options, default);
        return result.RecordsAffected;
    }

    protected static void TestConnectionBeforeRunningTests(string connectionString)
    {
        using var con = new DB2Connection(connectionString);
        foreach (var i in Enumerable.Range(1, 15))
        {
            try
            {
                con.Open();
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                {
                    break;
                }

                Thread.Sleep(60000);
            }
        }

        if (con.State != ConnectionState.Open)
        {
            throw new Exception("Check that the docker container is up and running.");
        }

        con.Close();
    }
}
#pragma warning restore SA1600 // Elements should be documented
