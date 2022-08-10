using BlogAPI.DTO;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ErrorService service;

        public ErrorController(ErrorService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Post error log
        /// * All errors are automatically saved to the DB in ErrorHandlerMiddleware *
        /// </summary>
        /// <param>ErrorItem</param>
        /// <returns>Action result</returns>
        [HttpPost("LogError")]
        public ActionResult LogError(ErrorItemDTO errorItem)
        {
            return Ok(service.PostError(errorItem));
        }

        /// <summary>
        /// Get list of the error log
        /// </summary>
        /// <returns>Mapped ErrorItem</returns>
        [HttpGet("GetAllErrors")]
        public ActionResult<ErrorItem[]> GetAllErrors()
        {
            return Ok(service.GetAllErrors());
        }

        /// <summary>
        /// Remove all error logs from the database
        /// </summary>
        [HttpPost("ClearErrors")]
        public ActionResult ClearErrors()
        {
            return Ok(service.DeleteAllErrors());
        }
    }
}
