using Microsoft.AspNet.Http;
using Microsoft.Extensions.Internal;

namespace Example.KendoUI.Extensions
{
    /// <summary>
    /// <see cref="HttpResponseExtensions"/> static class, provides extension methods for <see cref="HttpResponse"/> objects.
    /// </summary>
    public static class HttpResponseExtensions
    {
        #region Variables
        public const string ETagResponseHeader = "ETag";
        #endregion

        #region Methods
        /// <summary>
        /// Add ETag to the response header.
        /// </summary>
        /// <param name="response"><see cref="HttpResponse"/> object.</param>
        /// <param name="eTag">ETag value.</param>
        public static void AddETag(this HttpResponse response, [NotNull] string eTag)
        {
            response.Headers.Add(HttpResponseExtensions.ETagResponseHeader, new[] { eTag });
        }

        /// <summary>
        /// Checks to see if the response contains an ETag header.
        /// Returns true if it does.
        /// </summary>
        /// <param name="response"><see cref="HttpResponse"/> object.</param>
        /// <returns>Returns true if the response contains an ETag header.</returns>
        public static bool HasETag(this HttpResponse response)
        {
            return response.Headers.ContainsKey(HttpResponseExtensions.ETagResponseHeader);
        }

        /// <summary>
        /// Get the ETag response header value.
        /// </summary>
        /// <param name="response"><see cref="HttpResponse"/> object.</param>
        /// <returns>ETag value.</returns>
        public static string GetETag(this HttpResponse response)
        {
            return response.Headers[HttpResponseExtensions.ETagResponseHeader];
        }
        #endregion
    }
}
