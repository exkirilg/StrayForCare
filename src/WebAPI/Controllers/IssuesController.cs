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

    /// <summary>
    /// Creates new Issue
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">Request validation error</response>
    [HttpPost]
    public async Task<IActionResult> NewIssue(NewIssueRequest request)
    {
        await _issuesServices.NewIssueAsync(request);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok();
    }

    /// <summary>
    /// Changes Issue
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">Request validation error or no issue found by provided id</response>
    [HttpPut]
    public async Task<IActionResult> UpdateIssue(UpdateIssueRequest request)
    {
        await _issuesServices.UpdateIssueAsync(request);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok();
    }

    /// <summary>
    /// Marks Issue as deleted
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No issue found by provided id</response>
    [HttpPut("delete/{id}")]
    public async Task<IActionResult> SoftDeleteIssue(Guid id)
    {
        await _issuesServices.SoftDeleteIssueAsync(id);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok();
    }

    /// <summary>
    /// Deletes Issue from database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No issue found by provided id</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIssue(Guid id)
    {
        await _issuesServices.DeleteIssueAsync(id);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok();
    }
}
