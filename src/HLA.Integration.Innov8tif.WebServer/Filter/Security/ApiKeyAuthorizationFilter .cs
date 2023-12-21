using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using HLA.Backend.Core.Domain.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HLA.Integration.Innov8tif.WebServer.Filter.Security
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public class ApiKeyAuthorizationFilter : IAuthorizationFilter
    {
        private readonly IApiKeyValidator _apiKeyValidator;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
        {
            _apiKeyValidator = apiKeyValidator;
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string? apiKey = context.HttpContext.Request.Headers[HeaderName.ApiKey];
            string? clientId = context.HttpContext.Request.Headers[HeaderName.ClientId];

            if ((string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(clientId)) || !_apiKeyValidator.IsValid(clientId,apiKey))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Result = new JsonResult(null);
            }
        }
    }
}
