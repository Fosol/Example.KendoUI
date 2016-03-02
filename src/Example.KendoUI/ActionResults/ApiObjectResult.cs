using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Example.KendoUI.Extensions;

namespace Example.KendoUI.ActionResults
{
    public class ApiObjectResult : ObjectResult
    {
        #region Properties
        public string ETag { get; set; }
        #endregion

        #region Constructors
        public ApiObjectResult(object value)
            : base(value)
        {

        }

        public ApiObjectResult(object value, string eTag)
            : base(value)
        {
            this.ETag = eTag;
        }
        #endregion

        #region Methods
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (this.Value == null)
                this.StatusCode = StatusCodes.Status204NoContent;

            if (this.Value != null && !String.IsNullOrEmpty(this.ETag))
                context.HttpContext.Response.AddETag(this.ETag);

            return base.ExecuteResultAsync(context);
        }
        #endregion
    }
}
