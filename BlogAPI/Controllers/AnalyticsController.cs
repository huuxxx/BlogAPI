using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private const int DAYS_IN_WEEK = 7;
        private const string ASCII_TICK = "\u2713";
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
        /// Get daily visits for the last week
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet("GetWeekVisits")]
        public ActionResult<AnalyticsWeekVisits[]> GetWeekVisits()
        {
            try
            {
                var retVal = new List<AnalyticsWeekVisits>();
                var cultureInfo = new CultureInfo("en-US");

                for (int i = DAYS_IN_WEEK; i > 0; i--)
                {
                    AnalyticsWeekVisits tempDbObj = new();
                    tempDbObj.NameOfDay = DateTime.Now.AddDays(-(i - 1)).DayOfWeek.ToString();
                    var dateSelector = DateTime.ParseExact(DateTime.Now.AddDays(-(i - 1)).ToString("yyyy/MM/dd"), "yyyy/MM/dd", cultureInfo);
                    var query = from x in context.VisitorItem
                                where x.DateVisited == dateSelector
                                select x.DateVisited;
                    tempDbObj.VisitsInDay = query.Count();
                    retVal.Add(tempDbObj);
                }

                return Ok(retVal);
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Return last x number of visits
        /// </summary>
        /// <param name="numOfRecords">Number of records to request. Default: 10</param>
        /// <returns></returns>
        [HttpPost("GetLastVisits")]
        public ActionResult<List<VisitorItemConverted>> GetLastVisits(int numOfRecords = 10)
        {
            try
            {
                var query = context.VisitorItem.OrderByDescending(x => x.Id).Take(numOfRecords);
                List<VisitorItem> queryResults = query.ToList();
                queryResults.Reverse();
                List<VisitorItemConverted> retVal = new();

                for (int i = 0; i < queryResults.Count; i++)
                {
                    VisitorItemConverted tempItem = new();
                    tempItem.VisitorIP = queryResults[i].VisitorIP.ToString();
                    tempItem.DateVisited = queryResults[i].DateVisited.ToString("dd/MM/yyyy");                    
                    tempItem.ScreenHeight = queryResults[i].ScreenHeight.ToString();
                    tempItem.ScreenWidth = queryResults[i].ScreenWidth.ToString();
                    tempItem.ViewedAbout = queryResults[i].ViewedAbout ? ASCII_TICK : "";
                    tempItem.ViewedBlogs = queryResults[i].ViewedBlogs ? ASCII_TICK : "";
                    tempItem.ViewedProjects = queryResults[i].ViewedProjects ? ASCII_TICK : "";
                    retVal.Add(tempItem);
                }

                return Ok(retVal);
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
                var newVisitorItem = context.VisitorItem.OrderByDescending(x => x.Id).FirstOrDefault();
                int newId = newVisitorItem.Id;
                return Ok(newId);
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
        public async Task<IActionResult> PageViewed(PageViewed pageViewedItem)
        {
            try
            {
                var visitorItem = new VisitorItem { Id = pageViewedItem.SessionId };
                context.VisitorItem.Attach(visitorItem);

                switch (pageViewedItem.PageType)
                {
                    case "Blogs":
                        visitorItem.ViewedBlogs = true;
                        context.Entry(visitorItem).Property(x => x.ViewedBlogs).IsModified = true;
                        break;
                    case "Projects":
                        visitorItem.ViewedProjects = true;
                        context.Entry(visitorItem).Property(x => x.ViewedProjects).IsModified = true;
                        break;
                    case "About":
                        visitorItem.ViewedAbout = true;
                        context.Entry(visitorItem).Property(x => x.ViewedAbout).IsModified = true;
                        break;
                    default:
                        return BadRequest($"Invalid Page Type {pageViewedItem.PageType}");
                }

                await context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed for session ID {pageViewedItem.SessionId} \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }
    }
}
