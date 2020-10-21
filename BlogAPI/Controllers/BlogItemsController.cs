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

namespace BlogAPI.Controllers
{
    [Route("api/BlogItems")]
    [ApiController]
    public class BlogItemsController : ControllerBase
    {
        private readonly BlogItemContext blogItemContext;

        public BlogItemsController(BlogItemContext context)
        {
            blogItemContext = context;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<BlogItemDTO>>> GetBlogItem()
        //{
        //    return await _context.BlogItem
        //        .Select(x => BlogItemDTO(x))
        //        .ToListAsync();
        //}

        [HttpGet("{id}"), RequestLimit("Test-Action", NoOfRequest = 5, Seconds = 10), ValidateReferrer]
        public async Task<ActionResult<BlogItemDTO>> GetBlogItem(long id)
        {
            //var blogItem = await blogItemContext.BlogItem.FindAsync(id);

            //if (blogItem == null)
            //{
            //    return NotFound();
            //}

            //return BlogItemDTO(blogItem);

            string queryString = String.Format("SELECT * FROM [dbo].[BlogItem] WHERE ID = {0}", id);
            string connectionString = "Data Source=DESKTOP-2BP5LIO;" + "Initial Catalog=BlogAPI;" + "Integrated Security=SSPI;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
            }

            var blogItem = await blogItemContext.BlogItem.FindAsync(id);

            if (blogItem == null)
            {
                return NotFound();
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