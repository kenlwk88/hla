using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.Core.Domain.Auth;

namespace HLA.Backend.WebServer.Filter.Security
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
                context.Result = new JsonResult(Error.Response(901).TryCast<CommonResponse>());
            }
        }
    }
}
