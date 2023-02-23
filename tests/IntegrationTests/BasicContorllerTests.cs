using Microsoft.AspNetCore.Mvc;

namespace IntegrationTests;

public abstract class BasicContorllerTests : IClassFixture<TestDatabaseFixture>
{
    protected readonly TestDatabaseFixture _fixture;

    public BasicContorllerTests(TestDatabaseFixture fixture)
    {
        _fixture = fixture;
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
