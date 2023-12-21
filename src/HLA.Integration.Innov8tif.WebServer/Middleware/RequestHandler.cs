using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;


namespace HLA.Integration.Innov8tif.WebServer.Middleware
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public class RequestHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestHandler> _logger;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public RequestHandler(RequestDelegate next, IHostEnvironment env, ILogger<RequestHandler> logger)
        {
            _next = next;
            _logger = logger;
        }
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var isLog = context.Request.Path != "/";
                if (isLog) 
                {
                    await LogRequest(context.Request);
                }
                await _next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                var response = context.Response;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
        private async Task LogRequest(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            request.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            var message = $"{request.Scheme}://{request.Host}{request.Path} {request.QueryString} {bodyAsText}";
            _logger.LogInformation(message);
        }
    }
}
