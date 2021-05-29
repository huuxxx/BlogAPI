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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers
{
    [Route("api/BlogItems")]
    [ApiController]
    public class BlogItemsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public BlogItemsController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Get specific blog post by ID
        /// </summary>
        /// <param name="id">ID of blog item</param>
        /// <returns></returns>
        [HttpGet("GetBlog{id}"), RequestLimit("Test-Action", NoOfRequest = 5, Seconds = 10)]
        public async Task<ActionResult<BlogItemDTO>> GetBlog(long id)
        {
            try
            {
                string queryString = string.Format("SELECT * FROM [BlogItem] WHERE ID = {0}", id);

                string queryString1 = string.Format("UPDATE [BlogItem] SET Requests = ISNULL(Requests, 0) + 1 WHERE ID = {0}", id);

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                BlogItemDTO blogItemDTO = new BlogItemDTO();

                using (SqlConnection connection = new SqlConnection(connString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(queryString, connection);
                    
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        blogItemDTO.Id = (int)reader["ID"];
                        blogItemDTO.Title = reader["Title"].ToString();
                        blogItemDTO.Content = reader["Content"].ToString();
                        blogItemDTO.Requests = (int)reader["Requests"];
                        blogItemDTO.DateCreated = reader["DateCreated"].ToString();
                        blogItemDTO.DateModified = reader["DateModified"].ToString();
                    }

                    reader.Close();

                    SqlCommand command1 = new SqlCommand(queryString1, connection);

                    command1.ExecuteNonQuery();

                    connection.Close();
                }

                return blogItemDTO;
            }
            catch (Exception ex)
            {
                _ = ex;
                return BadRequest();
            }
        }

        /// <summary>
        /// Get latest blog
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetBlogLatest")]
        public async Task<ActionResult<BlogItemDTO>> GetBlogLatest()
        {
            try
            {
                string queryString = string.Format("SELECT * FROM [BlogItem] WHERE [ID] = (SELECT MAX(ID) FROM [BlogItem])");

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                BlogItemDTO blogItemDTO = new BlogItemDTO();

                await using (SqlConnection connection = new SqlConnection(connString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.Read())
                    {
                        blogItemDTO.Id = (int)reader["ID"];
                        blogItemDTO.Title = reader["Title"].ToString();
                        blogItemDTO.Content = reader["Content"].ToString();
                        blogItemDTO.Requests = (int)reader["Requests"] + 1;
                        blogItemDTO.DateCreated = reader["DateCreated"].ToString();
                        blogItemDTO.DateModified = reader["DateModified"].ToString();
                    }

                    reader.Close();

                    string queryString1 = string.Format("UPDATE [BlogItem] SET Requests = ISNULL(Requests, 0) + 1 WHERE ID = {0}", blogItemDTO.Id);

                    SqlCommand command1 = new SqlCommand(queryString1, connection);

                    command1.ExecuteNonQuery();

                    connection.Close();
                }

                return blogItemDTO;
            }
            catch (Exception ex)
            {
                _ = ex;
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Edit existing blog post
        /// </summary>
        /// <param name="id"></param>
        /// <param name="blogItemDTO"></param>
        /// <returns></returns>
        [HttpPut("EditBlog{id}"), Authorize]
        public async Task<IActionResult> EditBlog(long id, BlogItemDTO blogItemDTO)
        {
            try
            {
                string queryString = string.Format("UPDATE [BlogItem] SET Title = '{0}', Content = '{1}' WHERE ID = {2}", blogItemDTO.Title, blogItemDTO.Content, id);

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                await using (SqlConnection connection = new SqlConnection(connString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _ = ex;
                return BadRequest();
            }
        }

        /// <summary>
        /// Create new blog
        /// </summary>
        /// <param name="newBlogItemDTO"></param>
        /// <returns></returns>
        [HttpPost("CreateBlog"), Authorize]
        public async Task<ActionResult<NewBlogItemDTO>> CreateBlog(NewBlogItemDTO newBlogItemDTO)
        {
            try
            {
                string sqlEscapeTitle = Regex.Replace(newBlogItemDTO.Title, "'", "''");

                string sqlEscapeContent = Regex.Replace(newBlogItemDTO.Content, "'", "''");

                string queryString = string.Format("INSERT INTO [BlogItem] (Title, Content, DateCreated) VALUES (N'{0}', N'{1}', GetDate())", sqlEscapeTitle, sqlEscapeContent);

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                await using (SqlConnection connection = new SqlConnection(connString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _ = ex;
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete existing blog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteBlog{id}"), Authorize]
        public async Task<IActionResult> DeleteBlog(long id)
        {
            try
            {
                string queryString = string.Format("DELETE FROM [BlogItem] WHERE ID = {0}", id);

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                await using (SqlConnection connection = new SqlConnection(connString))
                {
                    SqlCommand command = new SqlCommand(queryString, connection);
                    connection.Open();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _ = ex;
                return BadRequest();
            }
        }
    }
}