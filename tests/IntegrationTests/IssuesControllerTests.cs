﻿using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Services.Issues;
using Services.Issues.Dto;
using System.Text;

namespace IntegrationTests;

public class IssuesControllerTests : BasicControllerTests<IssuesController>
{
    public IssuesControllerTests(TestDatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _controller = new IssuesController(new IssuesServices(_context));
    }

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
                && issue.Location.X == latitude
                && issue.Location.Y == longitude
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
}
