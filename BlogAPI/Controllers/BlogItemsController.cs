﻿using System;
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

        //[HttpGet("{id}"), RequestLimit("Test-Action", NoOfRequest = 5, Seconds = 10), ValidateReferrer]
        [HttpGet("{id}"), RequestLimit("Test-Action", NoOfRequest = 5, Seconds = 10)]
        public async Task<ActionResult<BlogItemDTO>> GetBlogItem(long id)
        {
            string queryString = string.Format("SELECT * FROM [dbo].[BlogItem] WHERE ID = {0}", id);

            string connString = ConfigurationExtensions.GetConnectionString(configuration, "BlogAPI");

            BlogItem blogItem = new BlogItem();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync();
                
                if (reader.Read())
                {
                    blogItem.Id = (int)reader["ID"];
                    blogItem.Title = reader["Title"].ToString();
                    blogItem.Content = reader["Content"].ToString();
                    connection.Close();
                }
            }

            return BlogItemDTO(blogItem);
        }

        //[HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogItem(long id, BlogItemDTO blogItemDTO)
        {
            if (id != blogItemDTO.Id)
            {
                return BadRequest();
            }

            var blogItem = await blogItemContext.BlogItem.FindAsync(id);
            if (blogItem == null)
            {
                return NotFound();
            }

            blogItem.Id = blogItemDTO.Id;
            blogItem.Title = blogItemDTO.Title;

            try
            {
                await blogItemContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!BlogItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        //[HttpPost]
        public async Task<ActionResult<BlogItemDTO>> CreateBlogItem(BlogItemDTO blogItemDTO)
        {
            var blogItem = new BlogItem
            {
                Id = blogItemDTO.Id,
                Title = blogItemDTO.Title,
                Content = blogItemDTO.Content
            };

            blogItemContext.BlogItem.Add(blogItem);
            await blogItemContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBlogItem),
                new { id = blogItem.Id },
                BlogItemDTO(blogItem));
        }

        //[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogItem(long id)
        {
            var blogItem = await blogItemContext.BlogItem.FindAsync(id);

            if (blogItem == null)
            {
                return NotFound();
            }

            blogItemContext.BlogItem.Remove(blogItem);
            await blogItemContext.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogItemExists(long id) =>
             blogItemContext.BlogItem.Any(e => e.Id == id);

        private static BlogItemDTO BlogItemDTO(BlogItem blogItem) =>
            new BlogItemDTO
            {
                Id = blogItem.Id,
                Title = blogItem.Title,
                Content = blogItem.Content
            };
    }
}