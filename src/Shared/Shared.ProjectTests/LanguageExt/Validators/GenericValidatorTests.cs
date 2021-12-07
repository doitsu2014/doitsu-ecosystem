using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Shared.LanguageExt.Common;
using Shared.LanguageExt.Validators;
using Xunit;

namespace Shared.Test.LanguageExt.Validators;

public class GenericValidatorTests
{
    [Fact]
    public void TestListGenericValidator()
    {
        var listData = new List<int>() { 1, 2, 3, 4, 5 };
        var listEmptyData = new List<object>();
        IEnumerable<object> listEmptyObject = null;

        Assert.True(listData.ShouldNotNullOrEmpty().IsSuccess);
        Assert.True(listEmptyData.ShouldNotNullOrEmpty().IsFail);
        Assert.True(listEmptyObject.ShouldNotNullOrEmpty().IsFail);
        Assert.True(listEmptyObject.ShouldNotNullOrEmpty().IsFail);
    }

    [Theory]
    [InlineData(10, 30)]
    [InlineData(20, 40)]
    [InlineData(30, 50)]
    [InlineData(40, 60)]
    public async Task Validation_MapAsync_Success(int num, int assertion)
    {
        var step = 10;
        var b = await num.ShouldNotNull()
            .MapAsync(x => Task.FromResult(x + step))
            .MapAsync(x =>
            {
                Thread.Sleep(100);
                return Task.FromResult(x + step);
            });
        Assert.True(b.IsSuccess);

        b.IfSuccess(value => Assert.True(value == assertion));
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(20, 20)]
    [InlineData(30, 30)]
    [InlineData(40, 40)]
    public async Task Validation_MapAsync_Fail(int num, int assertion)
    {
        var step = 10;
        var b = await num.ShouldNotNull()
            .MapAsync(x => Task.FromResult(x + step))
            .MapAsync(x =>
            {
                Thread.Sleep(100);
                return Task.FromResult(x + step);
            });
        Assert.True(b.IsSuccess);

        b.IfSuccess(value => Assert.True(value != assertion));
    }
}