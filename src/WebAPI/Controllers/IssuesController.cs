﻿using Microsoft.AspNetCore.Mvc;
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
    /// Returns collection of Issues with pagination
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">Request validation error</response>
    [HttpGet]
    public async Task<IActionResult> GetIssuesWithPagination([FromQuery] GetIssuesRequest request)
    {
        GetIssuesResponse? result = await _issuesServices.GetIssuesWithPagination(request);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok(result);
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
    /// Updates Issue
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
    /// Links together issue and tag
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No issue or no tag found by provided ids</response>
    [HttpPut("addTag")]
    public async Task<IActionResult> AddTagToIssue(AddTagToIssueRequest request)
    {
        await _issuesServices.AddTagToIssueAsync(request);

        if (_issuesServices.HasErrors)
            return this.ParseServicesErrorsToResult(_issuesServices);

        return Ok();
    }

    /// <summary>
    /// Breaks link between issue and tag
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No issue or no tag found by provided ids</response>
    [HttpPut("removeTag")]
    public async Task<IActionResult> RemoveTagFromIssue(RemoveTagFromIssueRequest request)
    {
        await _issuesServices.RemoveTagFromIssueAsync(request);

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
