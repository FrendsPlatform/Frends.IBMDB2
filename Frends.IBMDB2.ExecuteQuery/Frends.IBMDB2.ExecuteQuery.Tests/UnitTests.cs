namespace Frends.IBMDB2.ExecuteQuery.Tests
{
    using IBM.Data.Db2;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Frends.IBMDB2.ExecuteQuery.Definitions;
    using NUnit.Framework;

    /// <summary>
    /// To run Test run this docker command:
    /// docker run -h db2server --name db2server --restart=always --detach --privileged=true -p  50000:50000 --env-file ./lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2
    /// </summary>
    [TestFixture]
    internal class UnitTests
    {
        private static readonly string _connString = "Server=localhost:50000;Database=testdb;User Id=db2inst1;Password=password";
        private static readonly string _tableName = "TestTable";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            TestConnectionBeforeRunningTests(_connString);
        }

        [SetUp]
        public async Task Init()
        {
            var input = new Input()
            {
                ConnectionString = _connString,
                Query = $@"CREATE TABLE db2inst1.{_tableName} ( Id int, LASTNAME varchar(255), FIRSTNAME varchar(255) );",
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
                ConnectionString = _connString,
                Query = $@"DROP TABLE IF EXISTS db2inst1.{_tableName};",
                ExecuteType = ExecuteType.NonQuery,
                Parameters = null,
            };
            await IBMDB2.ExecuteQuery(input, new Options(), default);
        }

        [Test]
        public async Task TestExecuteQuery_Auto()
        {
            var transactionLevels = new List<TransactionIsolationLevel>()
        {
            TransactionIsolationLevel.Unspecified,
            TransactionIsolationLevel.Serializable,
            TransactionIsolationLevel.None,
            TransactionIsolationLevel.ReadUncommitted,
            TransactionIsolationLevel.ReadCommitted,
        };

            var inputInsert = new Input()
            {
                ConnectionString = _connString,
                Query = $@"INSERT INTO {_tableName} VALUES (1, 'Suku', 'Etu'), (2, 'Last', 'Forst'), (3, 'Hiiri', 'Mikki')",
                ExecuteType = ExecuteType.NonQuery,
                Parameters = null,
            };

            var inputSelect = new Input()
            {
                ConnectionString = _connString,
                Query = $"select * from {_tableName}",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            var inputSelectSingle = new Input()
            {
                ConnectionString = _connString,
                Query = $"select * from {_tableName} where Id = 1",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            var inputUpdate = new Input()
            {
                ConnectionString = _connString,
                Query = $@"update {_tableName} set LASTNAME = 'Edit' where Id = 2",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            var inputDelete = new Input()
            {
                ConnectionString = _connString,
                Query = $"delete from {_tableName} where Id = 2",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            foreach (var level in transactionLevels)
            {
                var options = new Options()
                {
                    SqlTransactionIsolationLevel = level,
                    ConnectionTimeoutSeconds = 2,
                    ThrowErrorOnFailure = true,
                };

                // Insert rows
                var insert = await IBMDB2.ExecuteQuery(inputInsert, options, default);
                Assert.IsTrue(insert.Success);
                Assert.AreEqual(3, insert.RecordsAffected);
                Assert.IsNull(insert.ErrorMessage);
                Assert.AreEqual(3, (int)insert.Data["AffectedRows"]);
                Assert.AreEqual(3, await GetRowCount()); // Make sure rows inserted before moving on.

                // Select all
                var select = await IBMDB2.ExecuteQuery(inputSelect, options, default);
                Assert.IsTrue(select.Success);
                Assert.AreEqual(-1, select.RecordsAffected);
                Assert.IsNull(select.ErrorMessage);
                Assert.AreEqual(typeof(JArray), select.Data.GetType());
                Assert.AreEqual("Suku", (string)select.Data[0]["LASTNAME"]);
                Assert.AreEqual("Etu", (string)select.Data[0]["FIRSTNAME"]);
                Assert.AreEqual("Last", (string)select.Data[1]["LASTNAME"]);
                Assert.AreEqual("Forst", (string)select.Data[1]["FIRSTNAME"]);
                Assert.AreEqual("Hiiri", (string)select.Data[2]["LASTNAME"]);
                Assert.AreEqual("Mikki", (string)select.Data[2]["FIRSTNAME"]);
                Assert.AreEqual(3, await GetRowCount()); // double check

                // Select single
                var selectSingle = await IBMDB2.ExecuteQuery(inputSelectSingle, options, default);
                Assert.IsTrue(selectSingle.Success);
                Assert.AreEqual(-1, selectSingle.RecordsAffected);
                Assert.IsNull(selectSingle.ErrorMessage);
                Assert.AreEqual(typeof(JArray), selectSingle.Data.GetType());
                Assert.AreEqual("Suku", (string)selectSingle.Data[0]["LASTNAME"]);
                Assert.AreEqual("Etu", (string)selectSingle.Data[0]["FIRSTNAME"]);
                Assert.AreEqual(3, await GetRowCount()); // double check

                // Update
                var update = await IBMDB2.ExecuteQuery(inputUpdate, options, default);
                Assert.IsTrue(update.Success);
                Assert.AreEqual(1, update.RecordsAffected);
                Assert.IsNull(update.ErrorMessage);
                Assert.AreEqual(3, await GetRowCount()); // double check
                var checkUpdateResult = await IBMDB2.ExecuteQuery(inputSelect, options, default);
                Assert.AreEqual("Suku", (string)checkUpdateResult.Data[0]["LASTNAME"]);
                Assert.AreEqual("Etu", (string)checkUpdateResult.Data[0]["FIRSTNAME"]);
                Assert.AreEqual("Edit", (string)checkUpdateResult.Data[1]["LASTNAME"]);
                Assert.AreEqual("Forst", (string)checkUpdateResult.Data[1]["FIRSTNAME"]);
                Assert.AreEqual("Hiiri", (string)checkUpdateResult.Data[2]["LASTNAME"]);
                Assert.AreEqual("Mikki", (string)checkUpdateResult.Data[2]["FIRSTNAME"]);
                Assert.AreEqual(3, await GetRowCount()); // double check

                // Delete
                var delete = await IBMDB2.ExecuteQuery(inputDelete, options, default);
                Assert.IsTrue(delete.Success);
                Assert.AreEqual(1, delete.RecordsAffected);
                Assert.IsNull(delete.ErrorMessage);
                Assert.AreEqual(2, await GetRowCount()); // double check
                var checkDeleteResult = await IBMDB2.ExecuteQuery(inputSelect, options, default);
                Assert.AreEqual("Suku", (string)checkDeleteResult.Data[0]["LASTNAME"]);
                Assert.AreEqual("Etu", (string)checkDeleteResult.Data[0]["FIRSTNAME"]);
                Assert.AreEqual("Hiiri", (string)checkDeleteResult.Data[1]["LASTNAME"]);
                Assert.AreEqual("Mikki", (string)checkDeleteResult.Data[1]["FIRSTNAME"]);

                await CleanUp();
                await Init();
            }
        }

        private static async Task<int> GetRowCount()
        {
            var input = new Input()
            {
                ConnectionString = _connString,
                Query = $"SELECT COUNT(*) FROM {_tableName};",
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

        private static void TestConnectionBeforeRunningTests(string connectionString)
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
                        break;

                    Thread.Sleep(60000);
                }
            }

            if (con.State != ConnectionState.Open)
                throw new Exception("Check that the docker container is up and running.");
            con.Close();
        }
    }
}
