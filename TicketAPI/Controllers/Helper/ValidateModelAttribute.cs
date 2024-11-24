using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TicketAPI.Controllers.Helper;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.Controller is Controller
            {
                ModelState.IsValid: false
            } controller)
        {
            var errors = controller.ModelState
                .Where(ms => ms.Value.Errors.Count > 0)
                .Select(ms => new
                {
                    Field = ms.Key,
                    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            filterContext.Result = new BadRequestObjectResult(errors);
        }
    }
}
