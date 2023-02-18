using Domain;
using IntegrationTests.TestData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [InlineData(0, 1, nameof(GetTagsRequest.PageSize))]
    [InlineData(0, 0, $"{nameof(GetTagsRequest.PageSize)},{nameof(GetTagsRequest.PageNum)}")]
    [InlineData(1, 0, nameof(GetTagsRequest.PageNum))]
    [InlineData(-1, 1, nameof(GetTagsRequest.PageSize))]
    [InlineData(-1, -1, $"{nameof(GetTagsRequest.PageSize)},{nameof(GetTagsRequest.PageNum)}")]
    [InlineData(1, -1, nameof(GetTagsRequest.PageNum))]
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

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public async Task GetTagById_ReturnsOk(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var expDto = TagsTestData.DtoData.Where(tag => tag.TagId == tagId).First();

        var result = (await controller.GetTagById(tagId)) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var dto = result.Value as TagDto;

        Assert.NotNull(dto);
        Assert.Equal(expDto, dto);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(8)]
    [InlineData(11)]
    public async Task GetTagById_ReturnsBadRequest(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var result = (await controller.GetTagById(tagId)) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.TagId)));
    }

    [Theory]
    [InlineData("Bat")]
    [InlineData("Cute turtle")]
    [InlineData("Raccoon")]
    public async Task NewTag_ReturnsOk(string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        
        var controller = new TagsController(new TagsServices(context));

        var request = new NewTagRequest(name);
        var result = (await controller.NewTag(request)) as StatusCodeResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        _ = context.Tags.Single(tag => tag.Name == name);
    }

    [Theory]
    [InlineData("Cat")]
    [InlineData("Dog")]
    public async Task NewTag_ReturnsBadRequest_NameIsNotUnique(string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var request = new NewTagRequest(name);

        var result = (await controller.NewTag(request)) as ObjectResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.Name)));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("Name is too long for a Tag!")]
    public async Task NewTag_ReturnsBadRequest_NameIsNotValid(string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var request = new NewTagRequest(name);

        var result = (await controller.NewTag(request)) as ObjectResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.Name)));
    }

    [Theory]
    [InlineData(1, "Bat")]
    [InlineData(3, "Cute turtle")]
    [InlineData(7, "Raccoon")]
    public async Task UpdateTagName_ReturnsOk(ushort tagId, string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var request = new UpdateTagNameRequest(tagId, name);
        var result = (await controller.UpdateTagName(request)) as StatusCodeResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        _ = context.Tags.Single(tag => tag.TagId == tagId && tag.Name == name);
    }

    [Theory]
    [InlineData(3, "Cat")]
    [InlineData(7, "Dog")]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotUnique(ushort tagId, string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var request = new UpdateTagNameRequest(tagId, name);
        var result = (await controller.UpdateTagName(request)) as ObjectResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.Name)));
    }

    [Theory]
    [InlineData(1, "")]
    [InlineData(3, " ")]
    [InlineData(2, "   ")]
    [InlineData(7, "Name is too long for a Tag!")]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotValid(ushort tagId, string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var request = new UpdateTagNameRequest(tagId, name);
        var result = (await controller.UpdateTagName(request)) as ObjectResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.Name)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public async Task SoftDeleteTag_ReturnsOk(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var result = (await controller.SoftDeleteTag(tagId)) as StatusCodeResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        Tag? tag;

        tag = context.Tags.FirstOrDefault(tag => tag.TagId == tagId);

        Assert.Null(tag);

        tag = context.Tags.IgnoreQueryFilters().Single(tag => tag.TagId == tagId);

        Assert.NotNull(tag);
        Assert.True(tag.SoftDeleted);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task SoftDeleteTag_ReturnsBadRequest(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var result = (await controller.SoftDeleteTag(tagId)) as ObjectResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.TagId)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public async Task DeleteTag_ReturnsOk(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var result = (await controller.DeleteTag(tagId)) as StatusCodeResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        Tag? tag;

        tag = context.Tags.FirstOrDefault(tag => tag.TagId == tagId);

        Assert.Null(tag);

        tag = context.Tags.IgnoreQueryFilters().FirstOrDefault(tag => tag.TagId == tagId);

        Assert.Null(tag);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task DeleteTag_ReturnsBadRequest(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();

        var controller = new TagsController(new TagsServices(context));

        var result = (await controller.DeleteTag(tagId)) as ObjectResult;

        context.ChangeTracker.Clear();

        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);

        var valResult = result.Value as Dictionary<string, object>;

        Assert.NotNull(valResult);
        Assert.True(valResult.ContainsKey(nameof(Tag.TagId)));
    }
}
