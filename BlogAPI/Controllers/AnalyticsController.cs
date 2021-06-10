using BlogAPI.DTO;
using BlogAPI.Models;
using BlogAPI.Types;
using Microsoft.AspNetCore.Http;
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
        /// Post initial site visitor info
        /// </summary>
        /// <param></param>
        /// <returns>Session ID</returns>
        [HttpPost("NewVisitor")]
        public async Task<ActionResult<int>> NewVisitor(VisitorItem visitorItem)
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

        /// <summary>
        /// Post page view info
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost("PageViewed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PageViewed(int sessionId, PageTypes pageViewed)
        {
            try
            {
                VisitorItem visitorItem = new();
                context.VisitorItem.Attach(visitorItem);

                switch (pageViewed)
                {
                    case PageTypes.Blogs:
                        context.Entry(visitorItem).Property(x => x.ViewedBlogs).IsModified = true;
                        break;
                    case PageTypes.Projects:
                        context.Entry(visitorItem).Property(x => x.ViewedProjects).IsModified = true;
                        break;
                    case PageTypes.About:
                        context.Entry(visitorItem).Property(x => x.ViewedAbout).IsModified = true;
                        break;
                    default:
                        return BadRequest($"Invalid Page Type {pageViewed}");
                }

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed for session ID {sessionId} \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }
    }
}
