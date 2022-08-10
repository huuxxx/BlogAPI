using BlogAPI.DTO;
using BlogAPI.Entities;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BlogAPI.Services
{
    public class BlogService : IBlogService
    {
        private readonly BlogContext context;
        private const string TestFileString = "TrapMoneyBrycey.jpg";
        private const string TestUrlString = "https://blogapi.huxdev.com/Images/TrapMoneyBrycey.jpg";
        private const string ImagesDirectory = @"wwwroot\Images\";

        public BlogService(BlogContext context)
        {
            this.context = context;
        }
        
        public async Task<int> CreateBlog(NewBlogItemDTO newBlogItemDTO)
        {
            BlogItem newBlog = new()
            {
                Content = Regex.Replace(newBlogItemDTO.Content, "'", "''"),
                Title = Regex.Replace(newBlogItemDTO.Title, "'", "''"),
                DateCreated = DateTime.Now.ToString("yyyy/MM/dd"),
                Id = context.BlogItem.OrderByDescending(x => x.Id).First().Id + 1,
            };
            
            context.BlogItem.Attach(newBlog);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteBlog(DeleteBlog deleteBlog)
        {
            BlogItem blogToDelete = new() { Id = deleteBlog.id };
            context.BlogItem.Attach(blogToDelete);
            context.BlogItem.Remove(blogToDelete);
            return await context.SaveChangesAsync();
        }

        public async Task<int> EditBlog(EditBlog editBlog)
        {
            var query = context.BlogItem.First(x => x.Id == editBlog.id);
            query.Content = Regex.Replace(editBlog.content, "'", "''");
            query.Title = Regex.Replace(editBlog.title, "'", "''");
            query.DateModified = DateTime.Now.ToString("yyyy/MM/dd");
            return await context.SaveChangesAsync();
        }

        public Task<List<BlogGetAllBlogsDTO>> GetAllBlogs()
        {
            var query = context.BlogItem.OrderByDescending(x => x.Id).ToList();

            List<BlogGetAllBlogsDTO> returnList = new();

            foreach (var item in query)
            {
                BlogGetAllBlogsDTO temp = new();
                temp.Id = item.Id;
                temp.Title = item.Title;
                temp.DateCreated = item.DateCreated;
                returnList.Add(temp);
            }

            return Task.FromResult(returnList);
        }

        public Task<BlogDTO> GetBlog(GetBlog getBlog)
        {
            var query = context.BlogItem.OrderByDescending(x => x.Id).Take(getBlog.Id).First();
            return MapBlog(query, getBlog.PreventIncrement);
        }

        public Task<BlogDTO> GetBlogById(GetBlog getBlog)
        {
            var query = context.BlogItem.First(x => x.Id == getBlog.Id);
            return MapBlog(query, getBlog.PreventIncrement);
        }

        public Task<int> GetBlogCount()
        {
            var query = context.BlogItem.Count();
            return Task.FromResult(query);
        }

        public Task<BlogDTO> GetBlogLatest(bool preventIncrement)
        {
            var query = context.BlogItem.OrderByDescending(x => x.Id).Take(1).First();
            return MapBlog(query, preventIncrement);
        }

        public async Task<string> UploadImage([FromForm] IFormFile file)
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
            return TestUrlString;
#endif
        }

        private async Task<BlogDTO> MapBlog(BlogItem blogItem, bool preventIncrement)
        {
            var query = context.BlogItem.First(x => x.Id == blogItem.Id);
            BlogItem blogTemp = query;

            if (!preventIncrement)
            {
                blogTemp.Requests++;
                await context.SaveChangesAsync();
            }

            BlogDTO blog = new();
            blog.Id = blogTemp.Id;
            blog.Title = blogTemp.Title;
            blog.Content = blogTemp.Content;
            blog.Requests = blogTemp.Requests;
            blog.DateCreated = blogTemp.DateCreated;
            blog.DateModified = blogTemp.DateModified;

            return blog;
        }

        public Task<bool> DeleteImage(DeleteImage deleteImage)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ImagesDirectory + deleteImage.id);
            File.Delete(path);
            return Task.FromResult(true);
        }

        public Task<string[]> GetAllImages()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), ImagesDirectory);
            string[] fileEntries = Directory.GetFiles(path);

            if (fileEntries.Length > 0)
            {
                for (int i = 0; i < fileEntries.Length; i++)
                {
                    fileEntries[i] = Path.GetFileName(fileEntries[i]);
                }
            }
#if RELEASE
            return Task.FromResult(fileEntries);
#endif
#if DEBUG
            string[] test = { TestFileString };
            return Task.FromResult(test);
#endif
        }
    }
}
