using HLA.Backend.Core.Domain.Auth;

namespace HLA.Integration.Innov8tif.WebServer.Filter.Security
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public interface IApiKeyValidator
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        bool IsValid(string clientId, string apiKey);
    }
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly AuthConfig _auth;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public ApiKeyValidator(AuthConfig auth)
        {
            _auth = auth;
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public bool IsValid(string clientId, string apiKey)
        {
            string? requestApiKey = _auth.Auth.Where(s => s.ClientId?.ToLower() == clientId.ToLower()).Select(y => y.ClientSecret).FirstOrDefault();
            if (requestApiKey != null && requestApiKey == apiKey)
                return true;
            return false;
        }
    }
}
