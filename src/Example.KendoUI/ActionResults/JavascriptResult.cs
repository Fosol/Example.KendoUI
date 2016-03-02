using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Internal;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.KendoUI.ActionResults
{
    /// <summary>
    /// <see cref="JavascriptResult"/> class, provides a way to return dynamically generated javascript content.
    /// </summary>
    public class JavascriptResult : ApiObjectResult
    {
        #region Properties

        /// <summary>
        /// get/set - The HTTP ContentType - defaults to "application/javascript".
        /// </summary>
        public MediaTypeHeaderValue ContentType { get; set; } = new MediaTypeHeaderValue("application/javascript") { Encoding = Encoding.UTF8 };
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a <see cref="JavascriptResult"/> class.
        /// </summary>
        /// <param name="value">Value to be returned as javascript.</param>
        public JavascriptResult(object value) : base(value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="JavascriptResult"/> class.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="eTag"></param>
        public JavascriptResult(object value, string eTag) : base(value, eTag)
        {
            this.ETag = eTag;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds the javascript to the response body.
        /// </summary>
        /// <param name="context"><see cref="ActionContext"/> object.</param>
        /// <returns><see cref="Task"/> object.</returns>
        public override Task ExecuteResultAsync([NotNull] ActionContext context)
        {
            var response = context.HttpContext.Response;
            var content_type = this.ContentType;
            response.ContentType = content_type.ToString();
            if (this.StatusCode != null)
                response.StatusCode = this.StatusCode.Value;

            using (var writer = new HttpResponseStreamWriter(response.Body, content_type.Encoding))
            {
                writer.Write(this.Value);
            }

            return Task.FromResult(true);
        }
        #endregion
    }
}
