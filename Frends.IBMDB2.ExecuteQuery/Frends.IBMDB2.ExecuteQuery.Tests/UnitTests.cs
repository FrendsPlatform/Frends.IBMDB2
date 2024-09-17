namespace Frends.IBMDB2.ExecuteQuery.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Frends.IBMDB2.ExecuteQuery.Definitions;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;

    /// <summary>
    /// To run Test run this docker command:
    /// docker run -h db2server --name db2server --restart=always --detach --privileged=true -p  50000:50000 --env-file ./lib/env_list.txt -v $PWD:/database icr.io/db2_community/db2
    /// </summary>
    [TestFixture]
    internal class UnitTests : TestsBase
    {
        [Test]
        public void ExceptionThrowsError()
        {
            var options = TestOptions();
            options.ThrowErrorOnFailure = true;
            var inputInsert = new Input
            {
                ConnectionString = ConnString,
                Query = $@"INSERT INTO {TableName} VALUES (@id, 'Suku', 'Etu')",
                ExecuteType = (ExecuteType)99,
                Parameters = TestParameters().ToArray(),
            };

            Assert.ThrowsAsync<Exception>(
                async () => await IBMDB2.ExecuteQuery(inputInsert, options, CancellationToken.None));
        }

        [Test]
        public async Task ExceptionReturnsFailedResult()
        {
            var options = TestOptions();
            options.ThrowErrorOnFailure = false;
            var inputInsert = new Input
            {
                ConnectionString = ConnString,
                Query = $@"INSERT INTO {TableName} VALUES (@id, 'Suku', 'Etu')",
                ExecuteType = (ExecuteType)99,
                Parameters = TestParameters().ToArray(),
            };

            var insert = await IBMDB2.ExecuteQuery(inputInsert, options, CancellationToken.None);
            Assert.IsFalse(insert.Success);
        }

        [Test]
        public async Task ParameterProvidedWithInput()
        {
            var options = TestOptions();
            var inputInsert = new Input
            {
                ConnectionString = ConnString,
                Query = $@"INSERT INTO {TableName} VALUES (@id, 'Suku', 'Etu')",
                ExecuteType = ExecuteType.NonQuery,
                Parameters = TestParameters().ToArray(),
            };

            var insert = await IBMDB2.ExecuteQuery(inputInsert, options, CancellationToken.None);
            Assert.IsTrue(insert.Success);
            Assert.AreEqual(1, insert.RecordsAffected);
            Assert.IsNull(insert.ErrorMessage);
            Assert.AreEqual(1, (int)insert.Data["AffectedRows"]);
            Assert.AreEqual(1, await GetRowCount()); // Make sure rows inserted before moving on.
        }

        [Test]
        public async Task ParameterWithNoAutoType()
        {
            var parameters = TestParameters();
            parameters[0].DataType = DataTypes.Integer;

            var options = TestOptions();

            var inputInsert = new Input
            {
                ConnectionString = ConnString,
                Query = $@"INSERT INTO {TableName} VALUES (@id, 'Suku', 'Etu')",
                ExecuteType = ExecuteType.NonQuery,
                Parameters = parameters.ToArray(),
            };

            var insert = await IBMDB2.ExecuteQuery(inputInsert, options, CancellationToken.None);
            Assert.IsTrue(insert.Success);
            Assert.AreEqual(1, insert.RecordsAffected);
            Assert.IsNull(insert.ErrorMessage);
            Assert.AreEqual(1, (int)insert.Data["AffectedRows"]);
            Assert.AreEqual(1, await GetRowCount()); // Make sure rows inserted before moving on.
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
                ConnectionString = ConnString,
                Query =
                    $@"INSERT INTO {TableName} VALUES (1, 'Suku', 'Etu'), (2, 'Last', 'Forst'), (3, 'Hiiri', 'Mikki')",
                ExecuteType = ExecuteType.NonQuery,
                Parameters = null,
            };

            var inputSelect = new Input()
            {
                ConnectionString = ConnString,
                Query = $"select * from {TableName}",
                ExecuteType = ExecuteType.ExecuteReader,
                Parameters = null,
            };

            var inputSelectSingle = new Input()
            {
                ConnectionString = ConnString,
                Query = $"select * from {TableName} where Id = 1",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            var inputUpdate = new Input()
            {
                ConnectionString = ConnString,
                Query = $@"update {TableName} set LASTNAME = 'Edit' where Id = 2",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            var inputDelete = new Input()
            {
                ConnectionString = ConnString,
                Query = $"delete from {TableName} where Id = 2",
                ExecuteType = ExecuteType.Auto,
                Parameters = null,
            };

            var options = TestOptions();

            foreach (var level in transactionLevels)
            {
                options.SqlTransactionIsolationLevel = level;

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
    }
}
