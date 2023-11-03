namespace Frends.IBMDB2.ExecuteQuery
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
    using Frends.IBMDB2.ExecuteQuery.Definitions;
    using IBM.Data.DB2.Core;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Main class of the Task.
    /// </summary>
    public static class IBMDB2
    {
        /// <summary>
        /// Frends Task for executing queries in IBMDB2.
        /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.IBMDB2.ExecuteQuery).
        /// </summary>
        /// <param name="input">Input parameters.</param>
        /// <param name="options">Options parameters.</param>
        /// <param name="cancellationToken">Cancellation token given by Frends.</param>
        /// <returns>Object { bool Success, int RecordsAffected, string ErrorMessage, dynamic data }.</returns>
        public static async Task<Result> ExecuteQuery([PropertyTab] Input input, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            Result result;

            using var connection = new DB2Connection(input.ConnectionString);
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = input.Query;

            if (input.Parameters != null)
            {
                foreach (var parameter in input.Parameters)
                {
                    if (parameter.DataType is DataTypes.Auto)
                        command.Parameters.Add(parameterName: parameter.Name, value: parameter.Value);
                    else
                    {
                        var dbType = (DB2Type)Enum.Parse(typeof(DB2Type), parameter.DataType.ToString());
                        var commandParameter = command.Parameters.Add(parameter.Name, dbType);
                        commandParameter.Value = parameter.Value;
                    }
                }
            }

            if (options.SqlTransactionIsolationLevel is TransactionIsolationLevel.None)
                result = await ExecuteHandler(input, options, command, cancellationToken);
            else
            {
                using var transaction = connection.BeginTransaction(GetIsolationLevel(options));
                command.Transaction = transaction;
                result = await ExecuteHandler(input, options, command, cancellationToken);
            }

            return result;
        }

        private static async Task<Result> ExecuteHandler(Input input, Options options, DB2Command command, CancellationToken cancellationToken)
        {
            Result result;
            object dataObject;
            dynamic dataReader = null;
            var table = new DataTable();

            try
            {
                switch (input.ExecuteType)
                {
                    case ExecuteType.Auto:
                        if (input.Query.ToLower().StartsWith("select"))
                        {
                            dataReader = command.ExecuteReader();
                            table.Load(dataReader);
                            result = new Result(true, dataReader.RecordsAffected, null, JToken.FromObject(table));
                            dataReader.Close();
                            break;
                        }

                        dataObject = command.ExecuteNonQuery();
                        result = new Result(true, (int)dataObject, null, JToken.FromObject(new { AffectedRows = dataObject }));
                        break;
                    case ExecuteType.NonQuery:
                        dataObject = command.ExecuteNonQuery();
                        result = new Result(true, (int)dataObject, null, JToken.FromObject(new { AffectedRows = dataObject }));
                        break;
                    case ExecuteType.Scalar:
                        dataReader = command.ExecuteScalar();
                        table.Load(dataReader);
                        result = new Result(true, dataReader.RecordsAffected, null, JToken.FromObject(table));
                        dataReader.Close();
                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (command.Transaction != null)
                    await command.Transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                if (dataReader != null && !dataReader.IsClosed)
                    await dataReader.CloseAsync();

                if (command.Transaction is null)
                {
                    if (options.ThrowErrorOnFailure)
                        throw new Exception("ExecuteHandler exception: 'Options.TransactionIsolationLevel = None', so there was no transaction rollback.", ex);
                    else
                        return new Result(false, 0, $"ExecuteHandler exception: 'Options.TransactionIsolationLevel = None', so there was no transaction rollback. {ex}", null);
                }
                else
                {
                    try
                    {
                        await command.Transaction.RollbackAsync(cancellationToken);
                    }
                    catch (Exception rollbackEx)
                    {
                        if (options.ThrowErrorOnFailure)
                            throw new Exception("ExecuteHandler exception: An exception occurred on transaction rollback.", rollbackEx);
                        else
                            return new Result(false, 0, $"ExecuteHandler exception: An exception occurred on transaction rollback. Rollback exception: {rollbackEx}. ||  Exception leading to rollback: {ex}", null);
                    }

                    if (options.ThrowErrorOnFailure)
                        throw new Exception("ExecuteHandler exception: (If required) transaction rollback completed without exception.", ex);
                    else
                        return new Result(false, 0, $"ExecuteHandler exception: (If required) transaction rollback completed without exception. {ex}.", null);
                }
            }
        }

        private static IsolationLevel GetIsolationLevel(Options options)
        {
            return options.SqlTransactionIsolationLevel switch
            {
                TransactionIsolationLevel.Unspecified => IsolationLevel.Unspecified,
                TransactionIsolationLevel.ReadUncommitted => IsolationLevel.ReadUncommitted,
                TransactionIsolationLevel.ReadCommitted => IsolationLevel.ReadCommitted,
                TransactionIsolationLevel.RepeatableRead => IsolationLevel.RepeatableRead,
                TransactionIsolationLevel.Serializable => IsolationLevel.Serializable,
                TransactionIsolationLevel.Snapshot => IsolationLevel.Snapshot,
                _ => IsolationLevel.ReadCommitted,
            };
        }
    }

}