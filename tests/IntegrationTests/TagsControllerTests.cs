using Domain.Models;
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

    [Fact]
    public async Task GetTagById_ReturnsOk()
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        IEnumerable<TagDto> tagsDto = context.Tags.Select(TagDto.FromTag).ToList();

        foreach (TagDto expDto in tagsDto)
        {
            var result = await controller.GetTagById(expDto.Id);
            var dto = EnsureCorrectOkObjectResultAndCorrectValue<TagDto>(result as ObjectResult);

            Assert.Equal(expDto, dto);
        }
    }

    [Fact]
    public async Task GetTagById_ReturnsBadRequest()
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var result = await controller.GetTagById(Guid.NewGuid());
        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Id));
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
    [InlineData("Bat")]
    [InlineData("Cute turtle")]
    [InlineData("Raccoon")]
    public async Task UpdateTagName_ReturnsOk(string name)
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        var controller = new TagsController(new TagsServices(context));

        Random rand = new();
        Guid id = context.Tags
            .Select(tag => tag.Id)
            .ToList()
            .ElementAt(rand.Next(context.Tags.Count()));

        var request = new UpdateTagNameRequest(id, name);
        
        var result = await controller.UpdateTagName(request);

        context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = context.Tags.Single(tag => tag.Id == id && tag.Name == name);
    }

    [Fact]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotUnique()
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        Random rand = new();
        List<Tag> tags = context.Tags.ToList();

        Guid id = tags
            .Select(tag => tag.Id)
            .ElementAt(rand.Next(tags.Count));

        string name = tags
            .Where(tag => tag.Id != id)
            .Select(tag => tag.Name)
            .First();

        var request = new UpdateTagNameRequest(id, name);
        var result = await controller.UpdateTagName(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("Name is too long for a Tag!")]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotValid(string name)
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        Random rand = new();
        Guid id = context.Tags
            .Select(tag => tag.Id)
            .ToList()
            .ElementAt(rand.Next(context.Tags.Count()));

        var request = new UpdateTagNameRequest(id, name);
        
        var result = await controller.UpdateTagName(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    [Fact]
    public async Task SoftDeleteTag_ReturnsOk()
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        var controller = new TagsController(new TagsServices(context));

        Random rand = new();
        
        for (int i = 0; i < context.Tags.Count() * 2; i++)
        {
            Guid id = context.Tags
                .Select(tag => tag.Id)
                .ToList()
                .ElementAt(rand.Next(context.Tags.Count()));

            var result = await controller.SoftDeleteTag(id);

            EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

            // No tag with query filter
            Assert.Null(
                context.Tags.FirstOrDefault(tag => tag.Id == id)
            );

            // There is a tag with query filter ignored
            Tag? tag = context.Tags.IgnoreQueryFilters().Single(tag => tag.Id == id);
            Assert.NotNull(tag);
            Assert.True(tag.SoftDeleted);
        }

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task SoftDeleteTag_ReturnsBadRequest()
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var result = await controller.SoftDeleteTag(Guid.NewGuid());

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Id));
    }

    [Fact]
    public async Task DeleteTag_ReturnsOk()
    {
        using var context = _fixture.CreateContext();
        context.Database.BeginTransaction();
        var controller = new TagsController(new TagsServices(context));

        foreach (Guid id in context.Tags.Select(tag => tag.Id).ToList())
        {
            var result = await controller.DeleteTag(id);

            EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

            // No tag with query filter
            Assert.Null(
                context.Tags.FirstOrDefault(tag => tag.Id == id)
            );

            // No tag with query filter ignored
            Assert.Null(
                context.Tags.IgnoreQueryFilters().FirstOrDefault(tag => tag.Id == id)
            );
        }

        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task DeleteTag_ReturnsBadRequest()
    {
        using var context = _fixture.CreateContext();
        var controller = new TagsController(new TagsServices(context));

        var result = await controller.DeleteTag(Guid.NewGuid());

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Id));
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
