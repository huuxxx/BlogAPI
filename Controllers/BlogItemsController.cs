using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;
using BlogAPI.DTO;

namespace BlogAPI.Controllers
{
    [Route("api/BlogItems")]
    [ApiController]
    public class BlogItemsController : ControllerBase
    {
        private readonly BlogItemContext _context;

        public BlogItemsController(BlogItemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogItemDTO>>> GetBlogItem()
        {
            return await _context.BlogItem
                .Select(x => BlogItemDTO(x))
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogItemDTO>> GetTodoItem(long id)
        {
            var todoItem = await _context.BlogItem.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return BlogItemDTO(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoItem(long id, BlogItemDTO blogItemDTO)
        {
            if (id != blogItemDTO.Id)
            {
                return BadRequest();
            }

            var blogItem = await _context.BlogItem.FindAsync(id);
            if (blogItem == null)
            {
                return NotFound();
            }

            blogItem.Id = blogItemDTO.Id;
            blogItem.Title = blogItemDTO.Title;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<BlogItemDTO>> CreateBlogItem(BlogItemDTO blogItemDTO)
        {
            var blogItem = new BlogItem
            {
                Id = blogItemDTO.Id,
                Title = blogItemDTO.Title,
                Content = blogItemDTO.Content
            };

            _context.BlogItem.Add(blogItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBlogItem),
                new { id = blogItem.Id },
                BlogItemDTO(blogItem));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogItem(long id)
        {
            var blogItem = await _context.BlogItem.FindAsync(id);

            if (blogItem == null)
            {
                return NotFound();
            }

            _context.BlogItem.Remove(blogItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(long id) =>
             _context.BlogItem.Any(e => e.Id == id);

        private static BlogItemDTO BlogItemDTO(BlogItem blogItem) =>
            new BlogItemDTO
            {
                Id = blogItem.Id,
                Title = blogItem.Title,
                Content = blogItem.Content
            };
    }
}