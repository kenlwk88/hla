using Microsoft.AspNetCore.Mvc;
using System.Net;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Filter.Security;
using HLA.Backend.WebServer.Filter.Validation;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.WebServer.Domain.Notification;
using HLA.Backend.WebServer.Domain.Register;

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
    [Route("api/register")]
    #region "Swagger"
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ValidationErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.InternalServerError)]
    #endregion
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterServices _services;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public RegisterController(ILogger<RegisterController> logger, IRegisterServices services)
        {
            _services = services;

        }
        /// <summary>
        /// Register Device Token for FCM
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(RegisterApiResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody] RegisterApiRequest request)
        {
            var result = await _services.PostAsync(request);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Disable Registered Device Token for FCM
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(RegisterApiResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Put([FromBody] RegisterApiRequest request)
        {
            var result = await _services.PutAsync(request);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
