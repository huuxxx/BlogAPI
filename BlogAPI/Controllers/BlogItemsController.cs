using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;
using BlogAPI.DTO;
using Security.Api.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BlogAPI.Controllers
{
    [Route("api/BlogItems")]
    [ApiController]
    public class BlogItemsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly BlogItemContext blogItemContext;

        public BlogItemsController(IConfiguration configuration, BlogItemContext context)
        {
            this.configuration = configuration;
            blogItemContext = context;
        }

        /// <summary>
        /// Get specific blog post by ID
        /// </summary>
        /// <param name="id">ID of blog item</param>
        /// <returns></returns>
        [HttpGet("GetBlog{id}"), RequestLimit("Test-Action", NoOfRequest = 5, Seconds = 10)]
        public async Task<ActionResult<BlogItemDTO>> GetBlog(long id)
        {
            string queryString = string.Format("SELECT * FROM [dbo].[BlogItem] WHERE ID = {0}", id);

            string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

            BlogItemDTO blogItemDTO = new BlogItemDTO();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                
                if (reader.Read())
                {
                    blogItemDTO.Id = (int)reader["ID"];
                    blogItemDTO.Title = reader["Title"].ToString();
                    blogItemDTO.Content = reader["Content"].ToString();
                    connection.Close();
                }
            }

            return blogItemDTO;
        }

        /// <summary>
        /// Get latest blog
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBlogLatest"), RequestLimit("Test-Action", NoOfRequest = 5, Seconds = 10)]
        public async Task<ActionResult<BlogItemDTO>> GetBlogLatest()
        {
            string queryString = string.Format("SELECT * FROM [BlogItem] WHERE [ID] = (SELECT MAX(ID) FROM [BlogItem])");

            string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

            BlogItemDTO blogItemDTO = new BlogItemDTO();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                if (reader.Read())
                {
                    blogItemDTO.Id = (int)reader["ID"];
                    blogItemDTO.Title = reader["Title"].ToString();
                    blogItemDTO.Content = reader["Content"].ToString();
                    connection.Close();
                }
            }

            return blogItemDTO;
        }

        [HttpPut("EditBlog{id}")]
        public async Task<IActionResult> EditBlog(long id, BlogItemDTO blogItemDTO)
        {
            // TODO
            return NoContent();
        }

        [HttpPost("CreateBlog")]
        public async Task<ActionResult<BlogItemDTO>> CreateBlog(BlogItemDTO blogItemDTO)
        {
            // TODO
            return NoContent();
        }

        [HttpDelete("DeleteBlog{id}")]
        public async Task<IActionResult> DeleteBlog(long id)
        {
            // TODO
            return NoContent();
        }

        private bool BlogItemExists(long id) =>
             blogItemContext.BlogItem.Any(e => e.Id == id);
    }
}