using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlogAPI.Interfaces;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private IAnalyticsService service;

        public AnalyticsController(IAnalyticsService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Get total site visitors
        /// </summary>
        [HttpGet("GetAnalytics")]
        public ActionResult<AnalyticsOverviewDTO> GetAnalytics()
        {
            return Ok(service.GetVisitorsCount());
        }

        /// <summary>
        /// Get daily visits for the last week
        /// </summary>
        [HttpGet("GetWeekVisits")]
        public ActionResult<AnalyticsVisitsInDayDTO[]> GetWeekVisits()
        {
            return Ok(service.GetVisitorsForEachDayThisWeek());
        }

        /// <summary>
        /// Return last x number of visits
        /// </summary>
        /// <param name="numOfRecords">Number of records to request. Default: 10</param>
        [HttpPost("GetLastVisits")]
        public ActionResult<List<VisitorItemDTO>> GetLastVisits(int numOfRecords = 10)
        {
            return Ok(service.GetLastVisits(numOfRecords));
        }

        /// <summary>
        /// Register new site visitor
        /// </summary>
        /// <returns>Session ID</returns>
        [HttpPost("NewVisitor")]
        public async Task<ActionResult<int>> NewVisitor(Visitor visitorItem)
        {
            return Ok(await service.RegisterNewVisitor(visitorItem, HttpContext.Connection.RemoteIpAddress.ToString()));
        }

        /// <summary>
        /// Post page view info
        /// </summary>
        [HttpPost("PageViewed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PageViewed(PageViewed pageViewedItem)
        {
            return Ok(await service.RegisterPageView(pageViewedItem));
        }
    }
}
