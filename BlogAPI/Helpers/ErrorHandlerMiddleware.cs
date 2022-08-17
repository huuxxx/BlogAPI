using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BlogAPI.Helpers;
using BlogAPI.DTO;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace BlogAPI.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration configuration;
        private const string SEARCH_VALUE = " in ";

        public ErrorHandlerMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case AppException:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                ErrorDto errorItem = new();
                errorItem.Id = Guid.NewGuid();
                int index = error.StackTrace.LastIndexOf(SEARCH_VALUE);
                errorItem.StackTrace = error.StackTrace.Substring(index + SEARCH_VALUE.Length) ?? "";        
                errorItem.Message = error.Message ?? "";
                string sqlEscapeStackTrace = Regex.Replace(errorItem.StackTrace, "'", "''");
                string sqlEscapeMessage = Regex.Replace(errorItem.Message, "'", "''");

                string queryString = string.Format("INSERT INTO [ErrorItem] (Id, DateCreated, StackTrace, Message) VALUES ('{0}', GetDate(), '{1}', '{2}')", errorItem.Id, sqlEscapeStackTrace, sqlEscapeMessage);
                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");
                
                using (SqlConnection connection = new(connString))
                {
                    connection.Open();
                    SqlCommand command = new(queryString, connection);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
