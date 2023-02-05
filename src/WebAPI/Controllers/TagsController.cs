using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Services.Tags;
using Services.Tags.Dto;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly DataContext _context;

    public TagsController(DataContext context)
    {
        _context = context;
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
        var service = new TagsServices(_context);
        
        await service.NewTagAsync(request);

        if (service.HasErrors)
        {
            foreach (var error in service.Errors)
            {
                ModelState.AddModelError(
                    string.Join(',', error.MemberNames),
                    error.ErrorMessage ?? string.Empty);
            }
            return ValidationProblem();
        }
        
        return Ok();
    }
}
