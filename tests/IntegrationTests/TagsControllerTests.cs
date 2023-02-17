using IntegrationTests.TestData;
using Microsoft.AspNetCore.Mvc;
using Services.Tags;
using Services.Tags.Dto;
using WebAPI.Controllers;

namespace IntegrationTests;

public class TagsControllerTests : IClassFixture<TestDatabaseFixture>
{
	private readonly TestDatabaseFixture _fixture;

	public TagsControllerTests(TestDatabaseFixture fixture)
	{
        _fixture = fixture;
	}

	[Theory]
	[InlineData(5, 1, 5, false)]
    [InlineData(5, 2, 2, false)]
    [InlineData(5, 3, 0, false)]
    [InlineData(5, 1, 5, true)]
    [InlineData(5, 2, 2, true)]
    [InlineData(5, 3, 0, true)]
    public async Task GetTagsWithPagination_ReturnsOk(int pageSize, int pageNum, int expNum, bool desc)
	{
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));
        GetTagsRequest request = new(pageSize, pageNum, Descending: desc);

        var result = (await controller.GetTagsWithPagination(request)) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var collection = result.Value as IEnumerable<TagDto>;

        Assert.NotNull(collection);
        Assert.Equal(expNum, collection.Count());

        IEnumerable<TagDto> expCollection;
        if (desc)
        {
            expCollection = TagsTestData.DtoData
                .OrderByDescending(tag => tag.Name)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize);
        }
        else
        {
            expCollection = TagsTestData.DtoData
                .OrderBy(tag => tag.Name)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize);
        }
        
        Assert.Equal(expCollection, collection);
    }

    [Theory]
    [InlineData("aDopT", "6")]
    [InlineData("cat", "1")]
    [InlineData("s", "3,5,7")]
    [InlineData("ss", "7")]
    [InlineData("", "1,2,3,4,5,6,7,8,9")]
    [InlineData("cute turtle", "")]
    public async Task GetTagsWithPagination_ReturnsOkWithNameSearch(string nameSearch, string expIds)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));
        GetTagsRequest request = new(TagsTestData.Data.Count, 1, nameSearch);

        var result = (await controller.GetTagsWithPagination(request)) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var collection = result.Value as IEnumerable<TagDto>;

        Assert.NotNull(collection);

        var expIdsCollection = expIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => ushort.Parse(id));
        
        var expCollection = TagsTestData.DtoData
            .Where(tag => expIdsCollection.Contains(tag.TagId))     
            .OrderBy(tag => tag.Name);

        Assert.Equal(expCollection, collection);
    }

    [Theory]
    [InlineData(0, 1, "PageSize")]
    [InlineData(0, 0, "PageSize,PageNum")]
    [InlineData(1, 0, "PageNum")]
    [InlineData(-1, 1, "PageSize")]
    [InlineData(-1, -1, "PageSize,PageNum")]
    [InlineData(1, -1, "PageNum")]
    public async Task GetTagsWithPagination_ReturnsBadRequest(int pageSize, int pageNum, string expValidationProps)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));
        GetTagsRequest request = new(pageSize, pageNum);

        var result = (await controller.GetTagsWithPagination(request)) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);

        foreach (var expProp in expValidationProps.Split(',', StringSplitOptions.None))
            Assert.True(valResult.ContainsKey(expProp));
    }
}
