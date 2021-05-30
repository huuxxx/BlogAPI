using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ILogger<AnalyticsController> logger;
        private string logMessage;
        private readonly VisitorContext context;

        public AnalyticsController(ILogger<AnalyticsController> logger, VisitorContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        /// <summary>
        /// Get total site visitors
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("GetAnalytics")]
        public ActionResult<AnalyticsOverview> GetAnalytics()
        {
            try
            {
                AnalyticsOverview overview = new();
                overview.TotalVisits = context.VisitorItem.Count();
                return Ok(overview);
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Post site visitor info
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("NewVisitor")]
        public async Task<ActionResult<VisitorItem>> NewVisitor(VisitorItem visitorItem)
        {
            try
            {
                visitorItem.VisitorIP = HttpContext.Connection.RemoteIpAddress.ToString();
                context.VisitorItem.Add(visitorItem);
                await context.SaveChangesAsync();
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
