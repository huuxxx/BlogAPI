﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.Models;
using BlogAPI.DTO;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.IO;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<BlogController> logger;
        private string logMessage;

        public BlogController(IConfiguration configuration, ILogger<BlogController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Get specific blog post by n'th value in descending order of date
        /// (1 = newest blog)
        /// </summary>
        /// <returns>Blog item</returns>
        [HttpPost("GetBlog")]
        public async Task<ActionResult<BlogItemDTO>> GetBlog(GetBlog getBlog)
        {
            try
            {
                // Select n'th blog
                string queryString = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER (ORDER BY DateCreated DESC) AS row_num ,ID, Title, Content, Requests, DateCreated, DateModified FROM [BlogItem]) AS sub WHERE row_num = {0}", getBlog.Id);

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                BlogItemDTO blogItemDTO = new();

                using (SqlConnection connection = new(connString))
                {
                    connection.Open();

                    SqlCommand command = new(queryString, connection);
                    
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

                    // Update blog request count
                    string queryString1 = string.Format("UPDATE [BlogItem] SET Requests = ISNULL(Requests, 0) + 1 WHERE ID = {0}", blogItemDTO.Id);

                    if (getBlog.PreventIncrement == false)
                    {
                        SqlCommand command1 = new(queryString1, connection);
                        command1.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                return blogItemDTO;
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed for blog ID: {getBlog.Id} \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Get all blog posts excluding content
        /// </summary>
        /// <returns>Blog - ID(s), Title(s), Date(s)</returns>
        [HttpGet("GetAllBlogs")]
        public async Task<ActionResult<List<BlogGetAllItem>>> GetAllBlogs()
        {
            try
            {
                string queryString = "SELECT id, title, dateCreated FROM [BlogItem] ORDER BY id DESC";

                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                List<BlogGetAllItem> blogItems = new();

                using (SqlConnection connection = new(connString))
                {
                    connection.Open();
                    SqlCommand command = new(queryString, connection);
                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    if (!reader.HasRows)
                    {
                        return Ok("No blogs found");
                    }

                    while (reader.Read())
                    {
                        BlogGetAllItem blogItem = new();
                        blogItem.Id = (int)reader["ID"];
                        blogItem.Title = reader["Title"].ToString();
                        blogItem.DateCreated = reader["DateCreated"].ToString();
                        blogItems.Add(blogItem);
                    }

                    reader.Close();
                    connection.Close();
                }

                return Ok(blogItems);
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Get latest blog
        /// </summary>
        /// <param name="preventIncrement">default: false. If true it will prevent an increase to the view count of the blog</param>
        /// <returns>Blog item</returns>
        [HttpGet("GetBlogLatest")]
        public async Task<ActionResult<BlogItemDTO>> GetBlogLatest(bool? preventIncrement = false)
        {
            try
            {
                string queryString = string.Format("SELECT TOP 1 * FROM [BlogItem] ORDER BY [DateCreated] DESC");
                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");
                BlogItemDTO blogItemDTO = new();

                await using (SqlConnection connection = new(connString))
                {
                    SqlCommand command = new(queryString, connection);
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

                    if (preventIncrement == false)
                    {
                        string queryString1 = string.Format("UPDATE [BlogItem] SET Requests = ISNULL(Requests, 0) + 1 WHERE ID = {0}", blogItemDTO.Id);
                        SqlCommand command1 = new(queryString1, connection);
                        command1.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                return blogItemDTO;
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get total number of blogs
        /// </summary>
        /// <returns>Blog count</returns>
        [HttpGet("GetBlogCount")]
        public async Task<ActionResult<int>> GetBlogCount()
        {
            try
            {
                string queryString = string.Format("SELECT COUNT(*) FROM [dbo].[BlogItem]");
                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");
                int blogCount;

                await using (SqlConnection connection = new(connString))
                {
                    SqlCommand command = new(queryString, connection);
                    connection.Open();
                    blogCount = (int)command.ExecuteScalar();
                    connection.Close();
                }

                return blogCount;
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Edit existing blog post
        /// </summary>
        /// <returns>Query result</returns>
        [HttpPost("EditBlog"), Authorize]
        public async Task<IActionResult> EditBlog(EditBlog editBlog)
        {
            try
            {
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string sqlEscapeTitle = Regex.Replace(editBlog.title, "'", "''");
                string sqlEscapeContent = Regex.Replace(editBlog.content, "'", "''");
                string queryString = string.Format("UPDATE [BlogItem] SET Title = N'{0}', Content = N'{1}', DateModified = '{2}' WHERE ID = {3}", sqlEscapeTitle, sqlEscapeContent, date, editBlog.id);
                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                await using (SqlConnection connection = new(connString))
                {
                    SqlCommand command = new(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed for blog ID: {editBlog.id} \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Create new blog
        /// </summary>
        /// <param name="newBlogItemDTO">New blog item</param>
        /// <returns>Query result</returns>
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
                    SqlCommand command = new(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete existing blog
        /// </summary>
        /// <returns>Query Result</returns>
        [HttpPost("DeleteBlog"), Authorize]
        public async Task<IActionResult> DeleteBlog(DeleteBlog deleteBlog)
        {
            try
            {
                string queryString = string.Format("DELETE FROM [BlogItem] WHERE ID = {0}", deleteBlog.id);
                string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

                await using (SqlConnection connection = new(connString))
                {
                    SqlCommand command = new(queryString, connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed for blog ID: {deleteBlog.id} \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }

        /// <summary>
        /// Upload an image to the server
        /// </summary>
        /// <param name="file">Image to be uploaded</param>
        /// <returns>String URL path of uploaded image</returns>
        [HttpPost("UploadImage")]
        public async Task<ActionResult<string>> UploadImage([FromForm]IFormFile file)
        {
            try
            {
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\" + timeStamp + Path.GetExtension(file.FileName));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
#if RELEASE
                return "https://blogapi.huxdev.com/Images/" + timeStamp + Path.GetExtension(file.FileName);
#endif
#if DEBUG
                return "https://blogapi.huxdev.com/Images/TrapMoneyBrycey.jpg"; // This is an img sitting on the server for testing locally
#endif
            }
            catch (Exception ex)
            {
                logMessage = $"{DateTime.UtcNow.ToLongTimeString()} {Extensions.Extensions.GetCurrentMethod()} Failed to upload image: {file.FileName} \n {ex.Message}";
                logger.LogInformation(logMessage);
                return BadRequest();
            }
        }
    }
}