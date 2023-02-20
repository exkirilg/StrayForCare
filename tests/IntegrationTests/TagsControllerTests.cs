using Domain;
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
	[InlineData(5, 1, false)]
    [InlineData(5, 2, false)]
    [InlineData(5, 3, false)]
    [InlineData(5, 1, true)]
    [InlineData(5, 2, true)]
    [InlineData(5, 3, true)]
    public async Task GetTagsWithPagination_ReturnsOk(int pageSize, int pageNum, bool desc)
	{
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));
        
        GetTagsRequest request = new(pageSize, pageNum, Descending: desc);

        IQueryable<Tag> query = context.Tags;
        if (desc) query = query.OrderByDescending(tag => tag.Name);
        else query = query.OrderBy(tag => tag.Name);
        query = query.Skip(pageSize * (pageNum - 1)).Take(pageSize);

        IEnumerable<TagDto> expCollection = query.Select(TagDto.FromTag).ToList();

        var result = await controller.GetTagsWithPagination(request);
        var collection = EnsureCorrectOkObjectResultAndCorrectValue<IEnumerable<TagDto>>(result as ObjectResult);

        Assert.Equal(expCollection.Count(), collection.Count());        
        Assert.Equal(expCollection, collection);
    }

    [Theory]
    [InlineData("aDopT")]
    [InlineData("cat")]
    [InlineData("s")]
    [InlineData("ss")]
    [InlineData("")]
    [InlineData("cute turtle")]
    public async Task GetTagsWithPagination_ReturnsOkWithNameSearch(string nameSearch)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));
        
        GetTagsRequest request = new(context.Tags.Count(), 1, nameSearch);

        IEnumerable<TagDto> expCollection = context.Tags
            .Where(tag => tag.Name.ToLower().Contains(nameSearch.ToLower()))
            .OrderBy(tag => tag.Name)
            .Select(TagDto.FromTag)
            .ToList();

        var result = await controller.GetTagsWithPagination(request);
        var collection = EnsureCorrectOkObjectResultAndCorrectValue<IEnumerable<TagDto>>(result as ObjectResult);

        Assert.Equal(expCollection, collection);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(-1, -1)]
    [InlineData(1, -1)]
    public async Task GetTagsWithPagination_ReturnsBadRequest(int pageSize, int pageNum)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));
        
        GetTagsRequest request = new(pageSize, pageNum);

        string? expProps = default;
        if (pageSize <= 0 && pageNum <= 0)
            expProps = $"{nameof(GetTagsRequest.PageSize)},{nameof(GetTagsRequest.PageNum)}";
        else if (pageSize <= 0)
            expProps = nameof(GetTagsRequest.PageSize);
        else if (pageNum <= 0)
            expProps = nameof(GetTagsRequest.PageNum);

        var result = await controller.GetTagsWithPagination(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, expProps);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(7)]
    public async Task GetTagById_ReturnsOk(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        TagDto expDto = context.Tags.Where(tag => tag.TagId == tagId).Select(TagDto.FromTag).First();

        var result = await controller.GetTagById(tagId);
        var dto = EnsureCorrectOkObjectResultAndCorrectValue<TagDto>(result as ObjectResult);

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

        var result = await controller.GetTagById(tagId);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.TagId));
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
        var result = await controller.NewTag(request);

        context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = context.Tags.Single(tag => tag.Name == name);
    }

    [Theory]
    [InlineData("Cat")]
    [InlineData("Dog")]
    public async Task NewTag_ReturnsBadRequest_NameIsNotUnique(string name)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var request = new NewTagRequest(name);

        var result = await controller.NewTag(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("Name is too long for a Tag!")]
    public async Task NewTag_ReturnsBadRequest_NameIsNotValid(string name)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var request = new NewTagRequest(name);

        var result = await controller.NewTag(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
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
        
        var result = await controller.UpdateTagName(request);

        context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = context.Tags.Single(tag => tag.TagId == tagId && tag.Name == name);
    }

    [Theory]
    [InlineData(3, "Cat")]
    [InlineData(7, "Dog")]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotUnique(ushort tagId, string name)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var request = new UpdateTagNameRequest(tagId, name);
        
        var result = await controller.UpdateTagName(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    [Theory]
    [InlineData(1, "")]
    [InlineData(3, " ")]
    [InlineData(2, "   ")]
    [InlineData(7, "Name is too long for a Tag!")]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotValid(ushort tagId, string name)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var request = new UpdateTagNameRequest(tagId, name);
        
        var result = await controller.UpdateTagName(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
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

        var result = await controller.SoftDeleteTag(tagId);

        context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        // No tag with query filter
        Assert.Null(
            context.Tags.FirstOrDefault(tag => tag.TagId == tagId)
        );

        // There is a tag with query filter ignored
        Tag? tag = context.Tags.IgnoreQueryFilters().Single(tag => tag.TagId == tagId);
        Assert.NotNull(tag);
        Assert.True(tag.SoftDeleted);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task SoftDeleteTag_ReturnsBadRequest(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var result = await controller.SoftDeleteTag(tagId);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.TagId));
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

        var result = await controller.DeleteTag(tagId);

        context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        // No tag with query filter
        Assert.Null(
            context.Tags.FirstOrDefault(tag => tag.TagId == tagId)
        );

        // No tag with query filter ignored
        Assert.Null(
            context.Tags.IgnoreQueryFilters().FirstOrDefault(tag => tag.TagId == tagId)
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public async Task DeleteTag_ReturnsBadRequest(ushort tagId)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var result = await controller.DeleteTag(tagId);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.TagId));
    }

    private TVal EnsureCorrectOkObjectResultAndCorrectValue<TVal>(ObjectResult? result)
        where TVal : class
    {
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);

        var value = result.Value as TVal;

        Assert.NotNull(value);

        return value;
    }

    private void EnsureCorrectOkStatusCodeResult(StatusCodeResult? result)
    {
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
    }

    private void EnsureCorrectBadRequestResult(ObjectResult? result, string? expProps = default)
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
