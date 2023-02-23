using Microsoft.AspNetCore.Mvc;
using Services.Issues;
using Services.Issues.Dto;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IssuesController : ControllerBase
{
    private readonly IIssuesServices _issuesServices;

    public IssuesController(IIssuesServices issuesServices)
    {
        _issuesServices = issuesServices;
    }

    /// <summary>
    /// Returns Issue with specified id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No Issue found by provided id</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetIssueById(Guid id)
    {
        IssueDto? result = await _issuesServices.GetIssueByIdAsync(id);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok(result);
    }
}
