using Microsoft.AspNetCore.Mvc;

namespace HLA.Integration.Innov8tif.WebServer.Filter.Security
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public ApiKeyAttribute()
            : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
