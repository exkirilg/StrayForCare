using Microsoft.AspNetCore.Mvc;
using Services.Tags;
using Services.Tags.Dto;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly ITagsServices _tagsServices;

    public TagsController(ITagsServices tagsServices)
    {
        _tagsServices = tagsServices;
    }

    /// <summary>
    /// Creates new Tag
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">Request validation error</response>
    [HttpPost]
    public async Task<IActionResult> NewTag(NewTagRequest request)
    {
        await _tagsServices.NewTagAsync(request);

        if (_tagsServices.HasErrors)
            return this.ParseServicesErrorsToResult(_tagsServices);
        
        return Ok();
    }

    /// <summary>
    /// Changes Tag's name
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">Request validation error or no tag found by provided id</response>
    [HttpPut]
    public async Task<IActionResult> UpdateTagName(UpdateTagNameRequest request)
    {
        await _tagsServices.UpdateTagNameAsync(request);

        if (_tagsServices.HasErrors)
            return this.ParseServicesErrorsToResult(_tagsServices);

        return Ok();
    }

    /// <summary>
    /// Marks Tag as deleted
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No tag found by provided id</response>
    [HttpPut("delete/{tagId}")]
    public async Task<IActionResult> SoftDeleteTag(ushort tagId)
    {
        await _tagsServices.SoftDeleteAsync(tagId);

        if (_tagsServices.HasErrors)
            return this.ParseServicesErrorsToResult(_tagsServices);

        return Ok();
    }

    /// <summary>
    /// Deletes Tag from database
    /// </summary>
    /// <param name="tagId"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    /// <response code="400">No tag found by provided id</response>
    [HttpDelete("{tagId}")]
    public async Task<IActionResult> DeleteTag(ushort tagId)
    {
        await _tagsServices.DeleteAsync(tagId);

        if (_tagsServices.HasErrors)
            return this.ParseServicesErrorsToResult(_tagsServices);

        return Ok();
    }
}
