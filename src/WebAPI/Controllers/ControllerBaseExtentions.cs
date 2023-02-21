using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers;

public static class ControllerBaseExtentions
{
    public static IActionResult ParseServicesErrorsToResult(
        this ControllerBase controller, IServicesErrors services)
    {
        foreach (var error in services.Errors)
        {
            controller.ModelState.AddModelError(
                string.Join(',', error.MemberNames),
                error.ErrorMessage ?? string.Empty);
        }

        return controller.BadRequest(controller.ModelState);
    }
}
