using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Extensions.Internal;
using System;

namespace Example.KendoUI.Extensions
{
    /// <summary>
    /// HttpRequestExtensions static class, provides extension methods for HttpRequest objects.
    /// </summary>
    public static class HttpRequestExtensions
    {
        #region Variables
        public const string ETagRequestHeader = "If-None-Match";
        #endregion

        #region Methods
        /// <summary>
        /// Determine if the request already has the current content or whether it has been modified.
        /// True if the requester has old content.
        /// </summary>
        /// <param name="request">HttpRequest object.</param>
        /// <param name="eTag">Unique ETag value of the current content.</param>
        /// <returns>True if the request currently has old content.</returns>
        public static bool IsModified(this HttpRequest request, string eTag)
        {
            var if_none_match = request.Headers[HttpRequestExtensions.ETagRequestHeader];

            if (!request.HasETag()
                || String.IsNullOrEmpty(if_none_match)
                || String.IsNullOrEmpty(eTag)
                || if_none_match != eTag)
                return true;

            return false;
        }

        /// <summary>
        /// Check to see if the request contains an ETag header value.
        /// </summary>
        /// <param name="request">HttpRequest object.</param>
        /// <returns>True if the request contains an ETag header.</returns>
        public static bool HasETag(this HttpRequest request)
        {
            if (String.IsNullOrEmpty(request.Headers[HttpRequestExtensions.ETagRequestHeader]))
                return false;

            return true;
        }

        /// <summary>
        /// Returns the executing request URI to ensure partial pages use their own host to get files.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="urlHelper"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Uri GetExecutingUri(this HttpRequest request, [NotNull] IUrlHelper urlHelper, string path = null)
        {
            var uri = new Uri($"{request.Scheme}://{request.Host}");

            if (String.IsNullOrEmpty(path))
                return new Uri(uri, urlHelper.Content("~"));

            return new Uri(uri, urlHelper.Content(path));
        }
        #endregion
    }
}
