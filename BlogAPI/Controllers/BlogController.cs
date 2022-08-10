using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using BlogAPI.Models;
using BlogAPI.Services;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly BlogService service;

        public BlogController(BlogService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Get specific blog post by n'th value in descending chronological order
        /// (1 = newest blog)
        /// </summary>
        [HttpPost("GetBlog")]
        public async Task<ActionResult<BlogDTO>> GetBlog(GetBlog getBlog)
        {
            return await service.GetBlog(getBlog);
        }

        /// <summary>
        /// Get specific blog post by ID
        /// </summary>
        /// <returns>Blog item</returns>
        [HttpPost("GetBlogId")]
        public async Task<ActionResult<BlogDTO>> GetBlogId(GetBlog getBlog)
        {
            return await service.GetBlog(getBlog);
        }

        /// <summary>
        /// Get all blog post titles
        /// </summary>
        /// <returns>List of all blogs (ID, Title, Date Created)</returns>
        [HttpGet("GetAllBlogs")]
        public async Task<ActionResult<List<BlogGetAllBlogsDTO>>> GetAllBlogs()
        {
            return await service.GetAllBlogs();
        }

        /// <summary>
        /// Get latest blog
        /// </summary>
        /// <param name="preventIncrement">default: false. If true it will prevent an increase to the view count of the blog</param>
        /// <returns>Blog item</returns>
        [HttpGet("GetBlogLatest")]
        public async Task<ActionResult<BlogDTO>> GetBlogLatest(bool? preventIncrement = false)
        {
            return await service.GetBlogLatest((bool)preventIncrement);
        }

        /// <summary>
        /// Get total number of blogs
        /// </summary>
        /// <returns>Blog count</returns>
        [HttpGet("GetBlogCount")]
        public async Task<ActionResult<int>> GetBlogCount()
        {
            return await service.GetBlogCount();
        }

        /// <summary>
        /// Edit existing blog post
        /// </summary>
        /// <returns>Query result</returns>
        [HttpPost("EditBlog"), Authorize]
        public async Task<ActionResult<int>> EditBlog(EditBlog editBlog)
        {
            return await service.EditBlog(editBlog);
        }

        /// <summary>
        /// Create new blog
        /// </summary>
        /// <param name="newBlogItemDTO">New blog item</param>
        /// <returns>Query result</returns>
        [HttpPost("CreateBlog"), Authorize]
        public async Task<ActionResult<int>> CreateBlog(NewBlogItemDTO newBlogItemDTO)
        {
            return await service.CreateBlog(newBlogItemDTO);
        }

        /// <summary>
        /// Delete existing blog
        /// </summary>
        /// <returns>Query Result</returns>
        [HttpPost("DeleteBlog"), Authorize]
        public async Task<ActionResult<int>> DeleteBlog(DeleteBlog deleteBlog)
        {
            return await service.DeleteBlog(deleteBlog);
        }

        /// <summary>
        /// Upload an image to the server
        /// </summary>
        /// <param name="file">Image to be uploaded</param>
        /// <returns>String URL path of uploaded image</returns>
        [HttpPost("UploadImage")]
        public async Task<ActionResult<string>> UploadImage([FromForm] IFormFile file)
        {
            return await service.UploadImage(file);
        }

        /// <summary>
        /// Delete an image from the server
        /// </summary>
        /// <param name="deleteImage">Image object containing the ID of the target</param>
        [HttpPost("DeleteImage"), Authorize]
        public async Task<ActionResult> DeleteImage(DeleteImage deleteImage)
        {
            await service.DeleteImage(deleteImage);
            return Ok();
        }

        /// <summary>
        /// Get all files from wwwroot/Images
        /// </summary>
        [HttpGet("GetAllImages")]
        public async Task<ActionResult<string[]>> GetAllImages()
        {
            return await service.GetAllImages();
        }
    }
}