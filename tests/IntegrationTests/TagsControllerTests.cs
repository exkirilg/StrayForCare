using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Tags;
using Services.Tags.Dto;
using WebAPI.Controllers;

namespace IntegrationTests;

public class TagsControllerTests : BasicControllerTests<TagsController>
{
	public TagsControllerTests(TestDatabaseFixture fixture)
	{
        _context = fixture.CreateContext();
        _controller = new TagsController(new TagsServices(_context));
    }

    #region Get with pagination

    [Theory]
	[InlineData(5, 1, false)]
    [InlineData(5, 3, false)]
    [InlineData(3, 1, true)]
    [InlineData(3, 3, true)]
    public async Task GetTagsWithPagination_ReturnsOk(int pageSize, int pageNum, bool desc)
	{
        GetTagsRequest request = new(pageSize, pageNum, Descending: desc);

        IQueryable<Tag> query = _context.Tags;
        if (desc) query = query.OrderByDescending(tag => tag.Name);
        else query = query.OrderBy(tag => tag.Name);
        query = query.Skip(pageSize * (pageNum - 1)).Take(pageSize);

        IEnumerable<TagDto> expCollection = query.Select(tag => new TagDto(tag)).ToList();
        int expTotalCount = _context.Tags.Count();

        var result = await _controller.GetTagsWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetTagsResponse>(result as ObjectResult);

        Assert.Equal(expCollection, response.Tags);
        Assert.Equal(expTotalCount, response.TotalCount);
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
        GetTagsRequest request = new(_context.Tags.Count(), 1, nameSearch);

        IEnumerable<TagDto> expCollection = _context.Tags
            .Where(tag => tag.Name.ToLower().Contains(nameSearch.ToLower()))
            .OrderBy(tag => tag.Name)
            .Select(tag => new TagDto(tag))
            .ToList();

        var result = await _controller.GetTagsWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetTagsResponse>(result as ObjectResult);

        Assert.Equal(expCollection, response.Tags);
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
        GetTagsRequest request = new(pageSize, pageNum);

        string? expProps = default;
        if (pageSize <= 0 && pageNum <= 0)
            expProps = $"{nameof(GetTagsRequest.PageSize)},{nameof(GetTagsRequest.PageNum)}";
        else if (pageSize <= 0)
            expProps = nameof(GetTagsRequest.PageSize);
        else if (pageNum <= 0)
            expProps = nameof(GetTagsRequest.PageNum);

        var result = await _controller.GetTagsWithPagination(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, expProps);
    }

    #endregion

    #region Get by id

    [Fact]
    public async Task GetTagById_ReturnsOk()
    {
        IEnumerable<TagDto> tagsDto = _context.Tags.Select(tag => new TagDto(tag)).ToList();

        foreach (TagDto expDto in tagsDto)
        {
            var result = await _controller.GetTagById(expDto.Id);
            var dto = EnsureCorrectOkObjectResultAndCorrectValue<TagDto>(result as ObjectResult);

            Assert.Equal(expDto, dto);
        }
    }

    [Fact]
    public async Task GetTagById_ReturnsBadRequest()
    {
        var result = await _controller.GetTagById(Guid.NewGuid());
        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Id));
    }

    #endregion

    #region New

    [Theory]
    [InlineData("Bat")]
    [InlineData("Cute turtle")]
    [InlineData("Raccoon")]
    public async Task NewTag_ReturnsOk(string name)
    {
        _context.Database.BeginTransaction();
        
        var request = new NewTagRequest(name);
        var result = await _controller.NewTag(request);

        _context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = _context.Tags.Single(tag => tag.Name == name);
    }

    [Theory]
    [InlineData("Cat")]
    [InlineData("Dog")]
    public async Task NewTag_ReturnsBadRequest_NameIsNotUnique(string name)
    {
        var request = new NewTagRequest(name);

        var result = await _controller.NewTag(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("Name is too long for a Tag!")]
    public async Task NewTag_ReturnsBadRequest_NameIsNotValid(string name)
    {
        var request = new NewTagRequest(name);

        var result = await _controller.NewTag(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    #endregion

    #region Update

    [Theory]
    [InlineData("Bat")]
    [InlineData("Cute turtle")]
    [InlineData("Raccoon")]
    public async Task UpdateTagName_ReturnsOk(string name)
    {
        _context.Database.BeginTransaction();
        
        Random rand = new();
        Guid id = _context.Tags
            .Select(tag => tag.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Tags.Count()));

        var request = new UpdateTagNameRequest(id, name);
        
        var result = await _controller.UpdateTagName(request);

        _context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = _context.Tags.Single(tag => tag.Id == id && tag.Name == name);
    }

    [Fact]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotUnique()
    {
        Random rand = new();
        List<Tag> tags = _context.Tags.ToList();

        Guid id = tags
            .Select(tag => tag.Id)
            .ElementAt(rand.Next(tags.Count));

        string name = tags
            .Where(tag => tag.Id != id)
            .Select(tag => tag.Name)
            .First();

        var request = new UpdateTagNameRequest(id, name);
        var result = await _controller.UpdateTagName(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("Name is too long for a Tag!")]
    public async Task UpdateTagName_ReturnsBadRequest_NameIsNotValid(string name)
    {
        Random rand = new();
        Guid id = _context.Tags
            .Select(tag => tag.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Tags.Count()));

        var request = new UpdateTagNameRequest(id, name);
        
        var result = await _controller.UpdateTagName(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Name));
    }

    #endregion

    #region Soft delete

    [Fact]
    public async Task SoftDeleteTag_ReturnsOk()
    {
        _context.Database.BeginTransaction();
        
        Random rand = new();
        
        for (int i = 0; i < _context.Tags.Count() * 2; i++)
        {
            Guid id = _context.Tags
                .Select(tag => tag.Id)
                .ToList()
                .ElementAt(rand.Next(_context.Tags.Count()));

            var result = await _controller.SoftDeleteTag(id);

            EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

            // No tag with query filter
            Assert.Null(
                _context.Tags.FirstOrDefault(tag => tag.Id == id)
            );

            // There is a tag with query filter ignored
            Tag? tag = _context.Tags.IgnoreQueryFilters().Single(tag => tag.Id == id);
            Assert.NotNull(tag);
            Assert.True(tag.SoftDeleted);
        }

        _context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task SoftDeleteTag_ReturnsBadRequest()
    {
        var result = await _controller.SoftDeleteTag(Guid.NewGuid());

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Id));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteTag_ReturnsOk()
    {
        _context.Database.BeginTransaction();
        
        foreach (Guid id in _context.Tags.Select(tag => tag.Id).ToList())
        {
            var result = await _controller.DeleteTag(id);

            EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

            // No tag with query filter
            Assert.Null(
                _context.Tags.FirstOrDefault(tag => tag.Id == id)
            );

            // No tag with query filter ignored
            Assert.Null(
                _context.Tags.IgnoreQueryFilters().FirstOrDefault(tag => tag.Id == id)
            );
        }

        _context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task DeleteTag_ReturnsBadRequest()
    {
        var result = await _controller.DeleteTag(Guid.NewGuid());

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Tag.Id));
    }

    #endregion
}
