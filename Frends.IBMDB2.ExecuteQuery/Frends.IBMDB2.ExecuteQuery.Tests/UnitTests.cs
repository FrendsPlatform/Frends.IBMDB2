namespace Frends.IBMDB2.ExecuteQuery.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Frends.IBMDB2.ExecuteQuery.Definitions;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using NUnit.Framework.Legacy;

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
            ClassicAssert.IsFalse(insert.Success);
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
            ClassicAssert.IsTrue(insert.Success);
            ClassicAssert.AreEqual(1, insert.RecordsAffected);
            ClassicAssert.IsNull(insert.ErrorMessage);
            ClassicAssert.AreEqual(1, (int)insert.Data["AffectedRows"]);
            ClassicAssert.AreEqual(1, await GetRowCount()); // Make sure rows inserted before moving on.
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
            ClassicAssert.IsTrue(insert.Success);
            ClassicAssert.AreEqual(1, insert.RecordsAffected);
            ClassicAssert.IsNull(insert.ErrorMessage);
            ClassicAssert.AreEqual(1, (int)insert.Data["AffectedRows"]);
            ClassicAssert.AreEqual(1, await GetRowCount()); // Make sure rows inserted before moving on.
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
                ClassicAssert.IsTrue(insert.Success);
                ClassicAssert.AreEqual(3, insert.RecordsAffected);
                ClassicAssert.IsNull(insert.ErrorMessage);
                ClassicAssert.AreEqual(3, (int)insert.Data["AffectedRows"]);
                ClassicAssert.AreEqual(3, await GetRowCount()); // Make sure rows inserted before moving on.

                // Select all
                var select = await IBMDB2.ExecuteQuery(inputSelect, options, default);
                ClassicAssert.IsTrue(select.Success);
                ClassicAssert.AreEqual(-1, select.RecordsAffected);
                ClassicAssert.IsNull(select.ErrorMessage);
                ClassicAssert.AreEqual(typeof(JArray), select.Data.GetType());
                ClassicAssert.AreEqual("Suku", (string)select.Data[0]["LASTNAME"]);
                ClassicAssert.AreEqual("Etu", (string)select.Data[0]["FIRSTNAME"]);
                ClassicAssert.AreEqual("Last", (string)select.Data[1]["LASTNAME"]);
                ClassicAssert.AreEqual("Forst", (string)select.Data[1]["FIRSTNAME"]);
                ClassicAssert.AreEqual("Hiiri", (string)select.Data[2]["LASTNAME"]);
                ClassicAssert.AreEqual("Mikki", (string)select.Data[2]["FIRSTNAME"]);
                ClassicAssert.AreEqual(3, await GetRowCount()); // double check

                // Select single
                var selectSingle = await IBMDB2.ExecuteQuery(inputSelectSingle, options, default);
                ClassicAssert.IsTrue(selectSingle.Success);
                ClassicAssert.AreEqual(-1, selectSingle.RecordsAffected);
                ClassicAssert.IsNull(selectSingle.ErrorMessage);
                ClassicAssert.AreEqual(typeof(JArray), selectSingle.Data.GetType());
                ClassicAssert.AreEqual("Suku", (string)selectSingle.Data[0]["LASTNAME"]);
                ClassicAssert.AreEqual("Etu", (string)selectSingle.Data[0]["FIRSTNAME"]);
                ClassicAssert.AreEqual(3, await GetRowCount()); // double check

                // Update
                var update = await IBMDB2.ExecuteQuery(inputUpdate, options, default);
                ClassicAssert.IsTrue(update.Success);
                ClassicAssert.AreEqual(1, update.RecordsAffected);
                ClassicAssert.IsNull(update.ErrorMessage);
                ClassicAssert.AreEqual(3, await GetRowCount()); // double check
                var checkUpdateResult = await IBMDB2.ExecuteQuery(inputSelect, options, default);
                ClassicAssert.AreEqual("Suku", (string)checkUpdateResult.Data[0]["LASTNAME"]);
                ClassicAssert.AreEqual("Etu", (string)checkUpdateResult.Data[0]["FIRSTNAME"]);
                ClassicAssert.AreEqual("Edit", (string)checkUpdateResult.Data[1]["LASTNAME"]);
                ClassicAssert.AreEqual("Forst", (string)checkUpdateResult.Data[1]["FIRSTNAME"]);
                ClassicAssert.AreEqual("Hiiri", (string)checkUpdateResult.Data[2]["LASTNAME"]);
                ClassicAssert.AreEqual("Mikki", (string)checkUpdateResult.Data[2]["FIRSTNAME"]);
                ClassicAssert.AreEqual(3, await GetRowCount()); // double check

                // Delete
                var delete = await IBMDB2.ExecuteQuery(inputDelete, options, default);
                ClassicAssert.IsTrue(delete.Success);
                ClassicAssert.AreEqual(1, delete.RecordsAffected);
                ClassicAssert.IsNull(delete.ErrorMessage);
                ClassicAssert.AreEqual(2, await GetRowCount()); // double check
                var checkDeleteResult = await IBMDB2.ExecuteQuery(inputSelect, options, default);
                ClassicAssert.AreEqual("Suku", (string)checkDeleteResult.Data[0]["LASTNAME"]);
                ClassicAssert.AreEqual("Etu", (string)checkDeleteResult.Data[0]["FIRSTNAME"]);
                ClassicAssert.AreEqual("Hiiri", (string)checkDeleteResult.Data[1]["LASTNAME"]);
                ClassicAssert.AreEqual("Mikki", (string)checkDeleteResult.Data[1]["FIRSTNAME"]);

                await CleanUp();
                await Init();
            }
        }
    }
}
