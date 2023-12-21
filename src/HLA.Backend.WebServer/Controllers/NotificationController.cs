using Microsoft.AspNetCore.Mvc;
using System.Net;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Filter.Security;
using HLA.Backend.WebServer.Filter.Validation;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.WebServer.Domain.Notification;

namespace HLA.Backend.WebServer.Controllers
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [ApiKey]
    [ApiController]
    [ValidationFilter]
    [Route("api/notification")]
    #region "Swagger"
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ValidationErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.InternalServerError)]
    #endregion
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _services;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public NotificationController(ILogger<NotificationController> logger, INotificationServices services)
        {
            _services = services;
        }
        /// <summary>
        /// Push Notification
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(NotificationApiResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] NotificationApiRequest request)
        {
            var result = await _services.PostAsync(request);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Update Notification
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Put([FromQuery] string AppSource, string CustKey, string Type, int Id)
        {
            var result = await _services.PutAsync(AppSource, CustKey, Type, Id);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Get Notification
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetNotificationApiResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get([FromQuery] string AppSource, string CustKey, string Types, int Id, bool IsHtmlEncoded = false, int? index = null, int?size = null, int? days = null)
        {
            var result = await _services.GetAsync(AppSource, CustKey, Types, Id, IsHtmlEncoded, index, size, days);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
