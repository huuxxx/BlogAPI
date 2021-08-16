using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        /// Post error log
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
    }
}
