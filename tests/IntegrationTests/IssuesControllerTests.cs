using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Services.Issues;
using Services.Issues.Dto;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests;

public class IssuesControllerTests : BasicControllerTests<IssuesController>
{
    public IssuesControllerTests(TestDatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _controller = new IssuesController(new IssuesServices(_context));
    }

    #region Get with pagination

    [Theory]
    [InlineData(5, 1, false)]
    [InlineData(5, 3, false)]
    [InlineData(3, 1, true)]
    [InlineData(3, 3, true)]
    public async Task GetIssuesWithPaginationSortedByCreatedAt_ReturnsOk(int pageSize, int pageNum, bool desc)
    {
        GetIssuesRequest request = new() { PageSize = pageSize, PageNum = pageNum, SortBy = "CreatedAt", Descending = desc };

        IQueryable<Issue> query = _context.Issues
            .Skip(pageSize * (pageNum - 1)).Take(pageSize);

        int expCount = query.Count();
        int expTotalCount = _context.Issues.Count();

        var result = await _controller.GetIssuesWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetIssuesResponse>(result as ObjectResult);

        Assert.Equal(expCount, response.Issues.Count());
        Assert.Equal(expTotalCount, response.TotalCount);

        DateTime lastCreatedAt = desc ? DateTime.MaxValue : default;
        foreach (var issue in response.Issues)
        {
            if (desc)
                Assert.True(issue.CreatedAt <= lastCreatedAt);
            else
                Assert.True(issue.CreatedAt >= lastCreatedAt);
            
            lastCreatedAt = issue.CreatedAt;
        }
    }

    [Theory]
    [InlineData(5, 1, false, 44.599478982826376, 33.456433940185825)]
    [InlineData(5, 3, false, 44.599478982826376, 33.456433940185825)]
    [InlineData(3, 1, true, 36.06138606707264, 138.0765888996556)]
    [InlineData(3, 3, true, 36.06138606707264, 138.0765888996556)]
    public async Task GetIssuesWithPaginationSortedByDistance_ReturnsOk(int pageSize, int pageNum, bool desc, double latitude, double longitude)
    {
        GetIssuesRequest request = new()
        {
            PageSize = pageSize, PageNum = pageNum, SortBy = "Distance",
            Descending = desc, CurrentLocationLatitude = latitude, CurrentLocationLongitude = longitude
        };

        IQueryable<Issue> query = _context.Issues
            .Skip(pageSize * (pageNum - 1)).Take(pageSize);

        int expCount = query.Count();
        int expTotalCount = _context.Issues.Count();

        var result = await _controller.GetIssuesWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetIssuesResponse>(result as ObjectResult);

        Assert.Equal(expCount, response.Issues.Count());
        Assert.Equal(expTotalCount, response.TotalCount);

        double lastDistance = desc ? double.MaxValue : default;
        foreach (var issue in response.Issues)
        {
            if (desc)
                Assert.True(issue.Distance <= lastDistance);
            else
                Assert.True(issue.Distance >= lastDistance);

            lastDistance = issue.Distance;
        }
    }

    [Theory]
    [InlineData(44.599478982826376, 33.456433940185825, 33.525392803570526, 44.616615916982816)]
    [InlineData(36.06138606707264, 138.0765888996556, 30.718260692320914, 114.46375925631736)]
    public async Task GetIssuesWithPaginationSortedByDistance_ReturnsClosest(double latitude, double longitude, double expLatitude, double expLongitude)
    {
        GetIssuesRequest request = new()
        {
            PageSize = 1,
            PageNum = 1,
            SortBy = "Distance",
            Descending = false,
            CurrentLocationLatitude = latitude,
            CurrentLocationLongitude = longitude
        };

        var result = await _controller.GetIssuesWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetIssuesResponse>(result as ObjectResult);

        Assert.Single(response.Issues, issue => issue.Latitude == expLatitude && issue.Longitude == expLongitude);
    }

    [Theory]
    [InlineData(44.599478982826376, 33.456433940185825, 30.395633523176564, -91.69046257720453)]
    [InlineData(36.06138606707264, 138.0765888996556, 30.395633523176564, -91.69046257720453)]
    public async Task GetIssuesWithPaginationSortedByDistance_ReturnsFarthest(double latitude, double longitude, double expLatitude, double expLongitude)
    {
        GetIssuesRequest request = new()
        {
            PageSize = 1,
            PageNum = 1,
            SortBy = "Distance",
            Descending = true,
            CurrentLocationLatitude = latitude,
            CurrentLocationLongitude = longitude
        };

        var result = await _controller.GetIssuesWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetIssuesResponse>(result as ObjectResult);

        Assert.Single(response.Issues, issue => issue.Latitude == expLatitude && issue.Longitude == expLongitude);
    }

    [Theory]
    [InlineData(44.599478982826376, 33.456433940185825, 0, 1560)]
    [InlineData(44.599478982826376, 33.456433940185825, 1, 2407)]
    [InlineData(44.599478982826376, 33.456433940185825, 2, 2465)]
    [InlineData(44.599478982826376, 33.456433940185825, 3, 7003)]
    [InlineData(44.599478982826376, 33.456433940185825, 4, 9997)]
    public async Task GetIssuesWithPagination_ReturnsCorrectDistance(double latitude, double longitude, int elIndex, int expDistance)
    {
        GetIssuesRequest request = new()
        {
            PageSize = 5,
            PageNum = 1,
            SortBy = "Distance",
            Descending = false,
            CurrentLocationLatitude = latitude,
            CurrentLocationLongitude = longitude
        };

        var result = await _controller.GetIssuesWithPagination(request);
        var response = EnsureCorrectOkObjectResultAndCorrectValue<GetIssuesResponse>(result as ObjectResult);

        var el = response.Issues.Skip(elIndex).First();
        int distance = (int)Math.Round(el.Distance / 1000, 0);

        Assert.Equal(expDistance, distance);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(-1, -1)]
    [InlineData(1, -1)]
    [InlineData(101, 1)]
    public async Task GetIssuesWithPagination_ReturnsBadRequest(int pageSize, int pageNum)
    {
        GetIssuesRequest request = new() { PageSize = pageSize, PageNum = pageNum };

        string? expProps = default;
        if ((pageSize <= 0 || pageSize > 100) && pageNum <= 0)
            expProps = $"{nameof(GetIssuesRequest.PageSize)},{nameof(GetIssuesRequest.PageNum)}";
        else if (pageSize <= 0 || pageSize > 100)
            expProps = nameof(GetIssuesRequest.PageSize);
        else if (pageNum <= 0)
            expProps = nameof(GetIssuesRequest.PageNum);

        var result = await _controller.GetIssuesWithPagination(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, expProps);
    }

    #endregion

    #region Get by id

    [Fact]
    public async Task GetIssueById_ReturnsOk()
    {
        IEnumerable<IssueDto> issuesDto = _context.Issues.Select(issue => new IssueDto(issue)).ToList();

        foreach (IssueDto expDto in issuesDto)
        {
            var result = await _controller.GetIssueById(expDto.Id);
            var dto = EnsureCorrectOkObjectResultAndCorrectValue<IssueDto>(result as ObjectResult);

            Assert.Equal(expDto, dto);
        }
    }

    [Fact]
    public async Task GetIssueById_ReturnsBadRequest()
    {
        var result = await _controller.GetIssueById(Guid.NewGuid());
        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Id));
    }

    #endregion

    #region New

    [Theory]
    [InlineData(
        "Stray cat!",
        33.5253883308217, 44.61661705781236,
        "")]
    [InlineData(
        "Dogs Playing Poker",
        -77.03653970869306, 38.89222289620729,
        "Dogs Playing Poker, by Cassius Marcellus Coolidge, refers collectively to an 1894 painting, a 1903 series of sixteen oil paintings commissioned by Brown & Bigelow to advertise cigars, and a 1910 painting.[1][unreliable source?] All eighteen paintings in the overall series feature anthropomorphized dogs, but the eleven in which dogs are seated around a card table have become well known in the United States as examples of kitsch art in home decoration.")]
    [InlineData(
        "Great Sphinx of Giza",
        31.137894802583624, 29.975909301791205,
        "The Great Sphinx of Giza is a limestone statue of a reclining sphinx, a mythical creature with the head of a human, and the body of a lion.[1] Facing directly from west to east, it stands on the Giza Plateau on the west bank of the Nile in Giza, Egypt. The face of the Sphinx appears to represent the pharaoh Khafre.")]
    public async Task NewIssue_ReturnsOk(string title, double latitude, double longitude, string description)
    {
        _context.Database.BeginTransaction();

        var request = new NewIssueRequest(title, latitude, longitude, description);
        var result = await _controller.NewIssue(request);

        _context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = _context.Issues
            .ToList()
            .Single(issue =>
                issue.Title == title
                && issue.Location.Y == latitude
                && issue.Location.X == longitude
                && issue.Description == description
            );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(251)]
    [InlineData(1027)]
    public async Task NewIssue_ReturnsBadRequest_TitleIsNotValid(int titleLength)
    {
        StringBuilder strBuilder = new();
        for (int i = 0; i < titleLength; i++)
            strBuilder.Append('.');

        var request = new NewIssueRequest(strBuilder.ToString(), 0, 0, string.Empty);

        var result = await _controller.NewIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Title));
    }

    [Theory]
    [InlineData(2501)]
    [InlineData(5000)]
    public async Task NewIssue_ReturnsBadRequest_DescriptionIsNotValid(int descriptionLength)
    {
        StringBuilder strBuilder = new();
        for (int i = 0; i < descriptionLength; i++)
            strBuilder.Append('.');

        var request = new NewIssueRequest("Test", 0, 0, strBuilder.ToString());

        var result = await _controller.NewIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Description));
    }

    [Theory]
    [InlineData(91)]
    [InlineData(-91)]
    public async Task NewIssue_ReturnsBadRequest_LatitudeIsNotValid(double latitude)
    {
        var request = new NewIssueRequest("Test", latitude, 0, string.Empty);

        var result = await _controller.NewIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, "latitude");
    }

    [Theory]
    [InlineData(181)]
    [InlineData(-181)]
    public async Task NewIssue_ReturnsBadRequest_LongitudeIsNotValid(double longitude)
    {
        var request = new NewIssueRequest("Test", 0, longitude, string.Empty);

        var result = await _controller.NewIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, "longitude");
    }

    #endregion

    #region Update

    [Theory]
    [InlineData(
        "Little angry bat!",
        30.581892679392492, 114.30187275909799,
        "Found very angry (possibly rabid) bat. What should I do?")]
    [InlineData(
        "Alligator",
        30.840029824446415, -91.57459647273839,
        "It's just an alligatoor, nothing unusual, move along.")]
    [InlineData(
        "Raccoon",
        40.13703517080637, -83.11514198958723,
        "Raccoon has stolen my garbage!")]
    public async Task UpdateIssue_ReturnsOk(string title, double latitude, double longitude, string description)
    {
        _context.Database.BeginTransaction();

        Random rand = new();
        Guid id = _context.Issues
            .Select(issue => issue.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Issues.Count()));

        var request = new UpdateIssueRequest(id, title, latitude, longitude, description);

        var result = await _controller.UpdateIssue(request);

        _context.ChangeTracker.Clear();

        EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

        _ = _context.Issues
            .ToList()
            .Single(issue =>
                issue.Title == title
                && issue.Location.Y == latitude
                && issue.Location.X == longitude
                && issue.Description == description
            );
    }

    [Fact]
    public async Task UpdateIssue_ReturnsBadRequest_NoIssueById()
    {
        var request = new UpdateIssueRequest(Guid.NewGuid(), "test", 0, 0, string.Empty);

        var result = await _controller.UpdateIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Id));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(251)]
    [InlineData(1027)]
    public async Task UpdateIssue_ReturnsBadRequest_TitleIsNotValid(int titleLength)
    {
        StringBuilder strBuilder = new();
        for (int i = 0; i < titleLength; i++)
            strBuilder.Append('.');

        Random rand = new();
        Guid id = _context.Issues
            .Select(issue => issue.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Issues.Count()));

        var request = new UpdateIssueRequest(id, strBuilder.ToString(), 0, 0, string.Empty);

        var result = await _controller.UpdateIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Title));
    }

    [Theory]
    [InlineData(2501)]
    [InlineData(5000)]
    public async Task UpdateIssue_ReturnsBadRequest_DescriptionIsNotValid(int descriptionLength)
    {
        StringBuilder strBuilder = new();
        for (int i = 0; i < descriptionLength; i++)
            strBuilder.Append('.');

        Random rand = new();
        Guid id = _context.Issues
            .Select(issue => issue.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Issues.Count()));

        var request = new UpdateIssueRequest(id, "test", 0, 0, strBuilder.ToString());

        var result = await _controller.UpdateIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Description));
    }

    [Theory]
    [InlineData(91)]
    [InlineData(-91)]
    public async Task UpdateIssue_ReturnsBadRequest_LatitudeIsNotValid(double latitude)
    {
        Random rand = new();
        Guid id = _context.Issues
            .Select(issue => issue.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Issues.Count()));

        var request = new UpdateIssueRequest(id, "test", latitude, 0, string.Empty);

        var result = await _controller.UpdateIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, "latitude");
    }

    [Theory]
    [InlineData(181)]
    [InlineData(-181)]
    public async Task UpdateIssue_ReturnsBadRequest_LongitudeIsNotValid(double longitude)
    {
        Random rand = new();
        Guid id = _context.Issues
            .Select(issue => issue.Id)
            .ToList()
            .ElementAt(rand.Next(_context.Issues.Count()));

        var request = new UpdateIssueRequest(id, "test", 0, longitude, string.Empty);

        var result = await _controller.UpdateIssue(request);

        EnsureCorrectBadRequestResult(result as ObjectResult, "longitude");
    }

    #endregion

    #region Soft delete

    [Fact]
    public async Task SoftDeleteIssue_ReturnsOk()
    {
        _context.Database.BeginTransaction();

        Random rand = new();

        for (int i = 0; i < _context.Issues.Count() * 2; i++)
        {
            Guid id = _context.Issues
                .Select(issue => issue.Id)
                .ToList()
                .ElementAt(rand.Next(_context.Issues.Count()));

            var result = await _controller.SoftDeleteIssue(id);

            EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

            // No issue with query filter
            Assert.Null(
                _context.Issues.FirstOrDefault(issue => issue.Id == id)
            );

            // There is an issue with query filter ignored
            Issue? issue = _context.Issues.IgnoreQueryFilters().Single(issue => issue.Id == id);
            Assert.NotNull(issue);
            Assert.True(issue.SoftDeleted);
        }

        _context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task SoftDeleteIssue_ReturnsBadRequest()
    {
        var result = await _controller.SoftDeleteIssue(Guid.NewGuid());

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Id));
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteIssue_ReturnsOk()
    {
        _context.Database.BeginTransaction();

        foreach (Guid id in _context.Issues.Select(issue => issue.Id).ToList())
        {
            var result = await _controller.DeleteIssue(id);

            EnsureCorrectOkStatusCodeResult(result as StatusCodeResult);

            // No issue with query filter
            Assert.Null(
                _context.Issues.FirstOrDefault(issue => issue.Id == id)
            );

            // No issue with query filter ignored
            Assert.Null(
                _context.Issues.IgnoreQueryFilters().FirstOrDefault(issue => issue.Id == id)
            );
        }

        _context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task DeleteIssue_ReturnsBadRequest()
    {
        var result = await _controller.DeleteIssue(Guid.NewGuid());

        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Id));
    }

    #endregion
}
