using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Example.KendoUI.Extensions
{
    /// <summary>
    /// <see cref="HttpResponseMessageExtensions"/> static class, provides extension methods for <see cref="HttpResponseMessage"/> objects.
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Check if the <see cref="HttpResponseMessage"/> object contains an ETag.
        /// </summary>
        /// <param name="response"><see cref="HttpResponseMessage"/> object.</param>
        /// <returns>True if the response contains an ETag.</returns>
        public static bool HasETag(this HttpResponseMessage response)
        {
            return response.Headers.ETag != null;
        }
    }
}
