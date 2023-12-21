using HLA.Integration.Innov8tif.WebServer.Application;
using HLA.Integration.Innov8tif.WebServer.Domain;
using HLA.Integration.Innov8tif.WebServer.Filter.Security;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HLA.Integration.Innov8tif.WebServer.Controllers
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    [ApiKey]
    [ApiController]
    [Route("api/ekyc")]
    public class EkycController : ControllerBase
    {
        private readonly IEkycServices _service;
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public EkycController(ILogger<EkycController> logger, IEkycServices service)
        {
            _service = service;
        }
        /// <summary>
        /// Get a unique ID required for tracing and tracking of eKYC transactions 
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("journeyid")]
        public async Task<IActionResult> PostJourneyId([FromBody] dynamic request)
        {
            var result = await _service.PostJourneyId(request);
            if (result == null)
                return StatusCode((int)HttpStatusCode.BadGateway, ErrorResponse.Unknown());
            return Ok(result);
        }
        /// <summary>
        /// Feature for optical character recognition of ID card
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("okayid")]
        public async Task<IActionResult> PostOkayId([FromBody] dynamic request)
        {
            var result = await _service.PostOkayId(request);
            if (result == null)
                return StatusCode((int)HttpStatusCode.BadGateway, ErrorResponse.Unknown());
            return Ok(result);
        }
        /// <summary>
        /// Feature for facial biometric verification and liveness detection
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("okayface")]
        public async Task<IActionResult> PostOkayFace(IFormCollection data)
        {
            var result = await _service.PostOkayFace(data);
            if (result == null)
                return StatusCode((int)HttpStatusCode.BadGateway, ErrorResponse.Unknown());
            return Ok(result);
        }
        /// <summary>
        /// Feature for document authenticity check of ID card
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route("okaydoc")]
        public async Task<IActionResult> PostOkayDoc([FromBody] dynamic request)
        {
            var result = await _service.PostOkayDoc(request);
            if (result == null)
                return StatusCode((int)HttpStatusCode.BadGateway, ErrorResponse.Unknown());
            return Ok(result);
        }
        /// <summary>
        /// Scorecard result/Overall eKYC result
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        [HttpGet]
        [Route("scorecard")]
        public async Task<IActionResult> GetScoreCard()
        {
            var result = await _service.GetScoreCard(Request.Query);
            if (result == null)
                return StatusCode((int)HttpStatusCode.BadGateway, ErrorResponse.Unknown());
            return Ok(result);
        }
    }
}