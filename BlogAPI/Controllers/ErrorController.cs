using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ErrorContext context;

        public ErrorController(ErrorContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Post error log - this is primarly for testing
        /// All errors are automatically saved to the DB in ErrorHandlerMiddleware
        /// </summary>
        /// <param>ErrorItem</param>
        /// <returns>Action result</returns>
        [HttpPost("LogError")]
        public async Task<ActionResult> LogError(ErrorItem errorItem)
        {
            context.ErrorItem.Add(errorItem);
            await context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get list of the error log
        /// </summary>
        /// <returns>ErrorItem[]</returns>
        [HttpGet("GetAllErrors")]
        public ActionResult<ErrorItem[]> GetAllErrors()
        {
            var returnVal = context.ErrorItem.ToList();

            if (returnVal.Count > 0)
            {
                returnVal.Reverse();
            }

            return Ok(returnVal);
        }
    }
}
