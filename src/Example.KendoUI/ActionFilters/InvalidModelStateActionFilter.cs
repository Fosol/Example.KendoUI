using Example.KendoUI.Extensions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Internal;

namespace Example.KendoUI.ActionFilters
{
    /// <summary>
    /// <see cref="InvalidModelStateActionFilter"/> sealed class, provides a common way to immediately return a <see cref="BadRequestObjectResult"/> object if the ModelState.IsValid == false.
    /// </summary>
    public sealed class InvalidModelStateActionFilter : ActionFilterAttribute
    {
        /// <summary>
        /// Return a <see cref="BadRequestObjectResult"/> object if the ModelState.IsValid == false.
        /// </summary>
        /// <param name="context">The current context of the action.</param>
        public override void OnActionExecuting([NotNull] ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState.ToErrorMessage());
            }
        }
    }
}
