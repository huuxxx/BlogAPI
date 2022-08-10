using BlogAPI.DTO;
using BlogAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogAPI.Interfaces
{
    public interface IBlogService
    {
        Task<BlogDTO> GetBlog(GetBlog getBlog);

        Task<BlogDTO> GetBlogById(GetBlog getBlog);

        Task<List<BlogGetAllBlogsDTO>> GetAllBlogs();

        Task<BlogDTO> GetBlogLatest(bool preventIncrement);

        Task<int> GetBlogCount();

        Task<int> EditBlog(EditBlog editBlog);

        Task<int> CreateBlog(NewBlogItemDTO newBlogItemDTO);

        Task<int> DeleteBlog(DeleteBlog deleteBlog);

        Task<string> UploadImage([FromForm] IFormFile file);

        Task<bool> DeleteImage(DeleteImage deleteImage);

        Task<string[]> GetAllImages();
    }
}
