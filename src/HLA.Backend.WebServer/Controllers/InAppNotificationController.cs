using Microsoft.AspNetCore.Mvc;
using System.Net;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Filter.Security;
using HLA.Backend.WebServer.Filter.Validation;
using HLA.Backend.WebServer.Domain.InAppPushNotification;
using HLA.Backend.WebServer.Application.Interfaces;

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
    [Route("api/inapp/notification")]
    #region "Swagger"
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ValidationErrorResponse), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.InternalServerError)]
    #endregion
    public class InAppNotificationController : ControllerBase
    {
        private readonly IInAppPushNotificationServices _services;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public InAppNotificationController(ILogger<InAppNotificationController> logger, IInAppPushNotificationServices services)
        {
            _services = services;
        }
        /// <summary>
        /// InApp Push Notification
        /// </summary>
        /// <remarks>
        /// <h2>Parameter Description</h2>
        /// <h3>cust_key</h3>
        /// Provide Agent Code when app_source is ECRM/ECRM_BANCA, provide Customer ID Number when app_source is HLA360
        /// <b></b>
        /// <h3>app_source</h3>
        /// HLA360,ECRM,ECRM_BANCA
        /// <b></b>
        /// <h3>title</h3>
        /// Show as title on the device
        /// <b></b>
        /// <h3>message</h3>
        /// Show as message on the device
        /// <b></b>
        /// <h3>data</h3>
        /// Additional information such as policy no, claim no, etc....
        /// <b></b>
        /// <h3>Sample Request Body</h3>
        /// { 
        ///     "cust_key": "TSRWRYRCSCFY",
        ///     "app_source": "HLA360", 
        ///     "title": "Welcome",
        ///     "message": "Hello World!",
        ///     "data": {
        ///         "policyNo": "12345",
        ///         "claimNo": "abc123",
        ///         "notificationId": "56",
        ///         "moduleType": "ECLAIM",
        ///     }
        /// }
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("push")]
        [ProducesResponseType(typeof(InAppPushNotificationApiResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Push([FromBody] InAppPushNotificationApiRequest request)
        {
            var result = await _services.PushNotificationAsync(request);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// InApp Push Notification (Test)
        /// </summary>
        /// <remarks>
        /// <h2>Parameter Description</h2>
        /// <h3>cust_key</h3>
        /// Provide Agent Code when app_source is ECRM/ECRM_BANCA, provide Customer ID Number when app_source is HLA360
        /// <b></b>
        /// <h3>app_source</h3>
        /// HLA360,ECRM,ECRM_BANCA
        /// <b></b>
        /// <h3>title</h3>
        /// Show as title on the device
        /// <b></b>
        /// <h3>message</h3>
        /// Show as message on the device
        /// <b></b>
        /// <h3>data</h3>
        /// Additional information such as policy no, claim no, etc....
        /// <b></b>
        /// <h3>Sample Request Body</h3>
        /// { 
        ///     "cust_key": "TSRWRYRCSCFY",
        ///     "app_source": "HLA360", 
        ///     "data": {
        ///         "title": "Welcome",
        ///         "message": "Hello World!",
        ///         "policyNo": "12345",
        ///         "claimNo": "abc123",
        ///         "notificationId": "56",
        ///         "moduleType": "ECLAIM",
        ///     }
        /// }
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("test/push")]
        [ProducesResponseType(typeof(CommonResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> TestPush([FromBody] InAppPushNotificationApiRequest request)
        {
            var result = await _services.TestPushNotificationAsync(request);
            if (result.Code != 0)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
