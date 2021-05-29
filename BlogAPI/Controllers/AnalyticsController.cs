using BlogAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AnalyticsController> logger;
        private string logMessage;

        public AnalyticsController(IConfiguration configuration, ILogger<AnalyticsController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Get total site visitors
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("GetAllVisits")]
        public async Task<ActionResult<BlogItemDTO>> GetAllVisits()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Post new visitor info
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("NewVisitor")]
        public async Task<ActionResult<VisitorItem>> NewVisitor(VisitorItem newVisitorDTO)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }
    }
}
