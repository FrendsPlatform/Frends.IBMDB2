#pragma warning disable SA1600 // Elements should be documented
using System;
using System.Threading;
using System.Threading.Tasks;
using Frends.IBMDB2.ExecuteQuery;
using Frends.IBMDB2.ExecuteQuery.Definitions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Frends.IBMDB2.ExecuteQuery.Tests;

[TestFixture]
internal class ErrorHandlerTest : TestsBase
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsAsync<NotSupportedException>(
            async () => await IBMDB2.ExecuteQuery(InvalidInput(), TestOptions(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = TestOptions();
        options.ThrowErrorOnFailure = false;
        var result = await IBMDB2.ExecuteQuery(InvalidInput(), options, CancellationToken.None);
        ClassicAssert.IsFalse(result.Success);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = TestOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsAsync<Exception>(
            async () => await IBMDB2.ExecuteQuery(InvalidInput(), options, CancellationToken.None));
        Assert.That(ex.Message, Contains.Substring(CustomErrorMessage));
    }

    private static Input InvalidInput() => new Input
    {
        ConnectionString = ConnString,
        Query = "SELECT 1 FROM SYSIBM.SYSDUMMY1",
        ExecuteType = (ExecuteType)99,
        Parameters = null,
    };
}
