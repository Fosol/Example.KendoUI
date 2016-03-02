using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Internal;
using Microsoft.AspNet.Mvc;

namespace Example.KendoUI.ActionFilters
{
    /// <summary>
    /// <see cref="nameof(UnhandledExceptionActionFilter)"/> class, provides a way to capture any unhandled exceptions that occur during execution of an API endpoint.
    /// </summary>
    public class UnhandledExceptionActionFilter : ExceptionFilterAttribute
    {
        /// <summary>
        /// When an exception occurs it will set the HttpContext result to return an <see cref="nameof(ApiErrorResult)"/>.
        /// </summary>
        /// <param name="context"><see cref="nameof(ExceptionContext)"/> object.</param>
        public override void OnException([NotNull] ExceptionContext context)
        {
            context.Result = new ObjectResult(context.Exception.Message);
        }

        /// <summary>
        /// When an exception occurs it will set the HttpContext result to return an <see cref="nameof(ApiErrorResult)"/>.
        /// </summary>
        /// <param name="context"><see cref="nameof(ExceptionContext)"/> object.</param>
        /// <returns>Async <see cref="nameof(Task)"/>.</returns>
        public override Task OnExceptionAsync([NotNull] ExceptionContext context)
        {
            this.OnException(context);
            return Task.FromResult(0);
        }
    }
}
