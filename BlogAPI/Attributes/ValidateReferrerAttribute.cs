using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;

namespace Security.Api.Filters
{
    /// <summary>    
    /// ActionFilterAttribute to validate referrer url    
    /// </summary>      
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ValidateReferrerAttribute : ActionFilterAttribute
    {
        private IConfiguration configuration;

        public ValidateReferrerAttribute(IConfiguration configuration)
        {
            this.configuration = configuration;
        }


        /// <summary>    
        /// Called when /[action executing].    
        /// </summary>    
        /// <param name="context">The action context.</param>    
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            configuration = (IConfiguration)context.HttpContext.RequestServices.GetService(typeof(IConfiguration));
            base.OnActionExecuting(context);
            if (!IsValidRequest(context.HttpContext.Request))
            {
                context.Result = new ContentResult
                {
                    Content = ($"Invalid referer header")
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
            }
        }

        /// <summary>    
        /// Determines whether /[is valid request] [the specified request].    
        /// </summary>    
        /// <param name="request">The request.</param>    
        /// <returns>    
        /// <c>true</c> if [is valid request] [the specified request]; otherwise, <c>false</c>.    
        /// </returns>    
        private bool IsValidRequest(HttpRequest request)
        {
            string referrerURL = "";

            if (request.Headers.ContainsKey("Referer"))
            {
                referrerURL = request.Headers["Referer"];
            }

            if (string.IsNullOrWhiteSpace(referrerURL)) return false;

            var allowedUrls = configuration.GetSection("CorsOrigin").Get<string[]>()?.Select(url => new Uri(url).Authority).ToList();
  
            var host = request.Host.Value;

            allowedUrls.Add(host);

            bool isValidClient = allowedUrls.Contains(new Uri(referrerURL).Authority);

            return isValidClient;
        }
    }
}