using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Services.Issues;
using Services.Issues.Dto;

namespace IntegrationTests;

public class IssuesControllerTests : BasicContorllerTests
{
    public IssuesControllerTests(TestDatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetIssueById_ReturnsOk()
    {
        using var context = _fixture.CreateContext();
        var controller = new IssuesController(new IssuesServices(context));

        IEnumerable<IssueDto> issuesDto = context.Issues.Select(issue => new IssueDto(issue)).ToList();

        foreach (IssueDto expDto in issuesDto)
        {
            var result = await controller.GetIssueById(expDto.Id);
            var dto = EnsureCorrectOkObjectResultAndCorrectValue<IssueDto>(result as ObjectResult);

            Assert.Equal(expDto, dto);
        }
    }

    [Fact]
    public async Task GetIssueById_ReturnsBadRequest()
    {
        using var context = _fixture.CreateContext();
        var controller = new IssuesController(new IssuesServices(context));

        var result = await controller.GetIssueById(Guid.NewGuid());
        EnsureCorrectBadRequestResult(result as ObjectResult, nameof(Issue.Id));
    }
}
