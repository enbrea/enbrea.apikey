#region Enbrea.ApiKey - Copyright (c) STÜBER SYSTEMS GmbH
/*    
 *    Enbrea.ApiKey
 *    
 *    Copyright (c) STÜBER SYSTEMS GmbH
 *
 *    Licensed under the MIT License, Version 2.0. 
 */
#endregion

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Enbrea.ApiKey
{
    /// <summary>
    /// A <see cref="ProblemDetails" /> based factory
    /// </summary>
    public class ApiKeyProblemDetailsFactory : IApiKeyErrorResultFactory
    {
        protected readonly ProblemDetailsFactory _problemDetailsFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyProblemDetailsFactory" /> class.
        /// </summary>
        /// <param name="problemDetailsFactory">Factory to produce <see cref="ProblemDetails" /></param>
        public ApiKeyProblemDetailsFactory(ProblemDetailsFactory problemDetailsFactory)
        {
            _problemDetailsFactory = problemDetailsFactory;
        }

        /// <summary>
        /// Creates an RFC 7807 <see cref="ProblemDetails"/> instance for a given <see cref="ApiKeyError"/>.
        /// </summary>
        /// <param name="httpContext">The current <see cref="HttpContext"/></param>
        /// <param name="error">The API key error classification</param>
        /// <returns>A <see cref="ProblemDetails"/> instance</returns>
        public virtual IActionResult Create(HttpContext httpContext, ApiKeyError error)
        {
            return error switch
            {
                ApiKeyError.Misconfigured => Problem(
                    httpContext,
                    statusCode: StatusCodes.Status500InternalServerError,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    title: "API key policy misconfigured"),

                ApiKeyError.PrivateOnlyDenied => Problem(
                    httpContext,
                    statusCode: StatusCodes.Status404NotFound,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    title: "Not found"),

                ApiKeyError.InvalidKey => Problem(
                    httpContext,
                    statusCode: StatusCodes.Status401Unauthorized,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    title: "Unauthorized"),

                _ => Problem(
                    httpContext,
                    statusCode: StatusCodes.Status500InternalServerError,
                    type: "https://tools.ietf.org/html/rfc9110#section-15.6.1", 
                    title: "Unexpected error")
            };
        }

        /// <summary>
        /// Creates an <see cref="ObjectResult"/> that produces a <see cref="ProblemDetails"/> response.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext" />.</param>
        /// <param name="statusCode">The value for <see cref="ProblemDetails.Status"/>.</param>
        /// <param name="title">The value for <see cref="ProblemDetails.Title" />.</param>
        /// <param name="type">The value for <see cref="ProblemDetails.Type" />.</param>
        /// <param name="detail">The value for <see cref="ProblemDetails.Detail" />.</param>
        /// <returns>The created <see cref="ObjectResult"/> for the response.</returns>
        protected ObjectResult Problem(
            HttpContext httpContext,
            int statusCode,
            string type,
            string title,
            string detail = null)
        {
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext,
                statusCode: statusCode,
                title: title,
                type: type,
                detail: detail,
                instance: httpContext.Request.Path);

            return new ObjectResult(problemDetails) { StatusCode = statusCode };
        }
    }
}