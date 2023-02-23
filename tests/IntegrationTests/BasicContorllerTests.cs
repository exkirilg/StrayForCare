using DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTests;

public abstract class BasicControllerTests<TController>
    : IDisposable, IClassFixture<TestDatabaseFixture>
    where TController : ControllerBase
{
    protected DataContext _context = null!;
    protected TController _controller = null!;

    public void Dispose()
    {
        _context.Dispose();
    }

    protected TVal EnsureCorrectOkObjectResultAndCorrectValue<TVal>(ObjectResult? result)
        where TVal : class
    {
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var value = result.Value as TVal;

        Assert.NotNull(value);

        return value;
    }

    protected void EnsureCorrectOkStatusCodeResult(StatusCodeResult? result)
    {
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    protected void EnsureCorrectBadRequestResult(ObjectResult? result, string? expProps = default)
    {
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        if (expProps is null) return;

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        foreach (var expProp in expProps.Split(',', StringSplitOptions.None))
            Assert.True(valResult.ContainsKey(expProp));
    }
}
