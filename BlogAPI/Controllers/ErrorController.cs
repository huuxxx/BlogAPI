using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration configuration;

        public ErrorController(ErrorContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        /// <summary>
        /// Post error log
        /// * All errors are automatically saved to the DB in ErrorHandlerMiddleware *
        /// </summary>
        /// <param>ErrorItem</param>
        /// <returns>Action result</returns>
        [HttpPost("LogError")]
        public async Task<ActionResult> LogError(ErrorItemDTO errorItem)
        {
            context.ErrorItem.Add(errorItem);
            await context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Get list of the error log
        /// </summary>
        /// <returns>Mapped ErrorItem</returns>
        [HttpGet("GetAllErrors")]
        public ActionResult<ErrorItem[]> GetAllErrors()
        {
            var queryList = context.ErrorItem.ToList();
            List<ErrorItem> returnList = new();

            if (queryList.Count > 0)
            {
                foreach (var item in queryList)
                {
                    ErrorItem temp = new();
                    temp.Id = item.Id;
                    temp.DateCreated = item.DateCreated.ToString("dd MMM yyyy hh:mmtt");
                    temp.StackTrace = item.StackTrace;
                    temp.Message = item.Message;
                    returnList.Add(temp);
                }
            }

            return Ok(returnList);
        }

        /// <summary>
        /// Remove all error logs from the database
        /// </summary>
        [HttpPost("ClearErrors")]
        public ActionResult ClearErrors()
        {
            string queryString = "TRUNCATE TABLE [ErrorItem]";
            string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");
            using (SqlConnection connection = new(connString))
            {
                connection.Open();
                SqlCommand command = new(queryString, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            return Ok();
        }
    }
}
