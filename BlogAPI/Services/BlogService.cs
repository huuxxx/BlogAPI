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
        
        public async Task<int> CreateBlog(NewBlogItemDto newBlogItemDTO)
        {
            int latestId;

            if (!context.BlogItem.Any())
            {
                latestId = 0;
            }
            else
            {
                latestId = context.BlogItem.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            }

            Blog newBlog = new()
            {
                Content = Regex.Replace(newBlogItemDTO.Content, "'", "''"),
                Title = Regex.Replace(newBlogItemDTO.Title, "'", "''"),
                DateCreated = DateTime.Now,
                Id = latestId,
            };
            
            context.BlogItem.Add(newBlog);
            return await context.SaveChangesAsync();
        }

        public async Task<int> DeleteBlog(DeleteBlog deleteBlog)
        {
            Blog blogToDelete = new() { Id = deleteBlog.id };
            context.BlogItem.Attach(blogToDelete);
            context.BlogItem.Remove(blogToDelete);
            return await context.SaveChangesAsync();
        }

        public async Task<int> EditBlog(EditBlog editBlog)
        {
            var query = context.BlogItem.First(x => x.Id == editBlog.id);
            query.Content = Regex.Replace(editBlog.content, "'", "''");
            query.Title = Regex.Replace(editBlog.title, "'", "''");
            query.DateModified = DateTime.Now;
            return await context.SaveChangesAsync();
        }

        public Task<List<BlogGetAllBlogsDto>> GetAllBlogs()
        {
            var query = context.BlogItem.OrderByDescending(x => x.Id).ToList();

            List<BlogGetAllBlogsDto> returnList = new();

            foreach (var item in query)
            {
                BlogGetAllBlogsDto temp = new();
                temp.Id = item.Id;
                temp.Title = item.Title;
                temp.DateCreated = item.DateCreated.ToString("yyyy/MM/dd");
                returnList.Add(temp);
            }

            return Task.FromResult(returnList);
        }

        public Task<BlogDto> GetBlog(GetBlog getBlog)
        {
            var query = context.BlogItem.OrderByDescending(x => x.Id).Take(getBlog.Id).First();
            return MapBlog(query, getBlog.PreventIncrement);
        }

        public Task<BlogDto> GetBlogById(GetBlog getBlog)
        {
            var query = context.BlogItem.First(x => x.Id == getBlog.Id);
            return MapBlog(query, getBlog.PreventIncrement);
        }

        public Task<int> GetBlogCount()
        {
            var query = context.BlogItem.Count();
            return Task.FromResult(query);
        }

        public Task<BlogDto> GetBlogLatest(bool preventIncrement)
        {
            var query = context.BlogItem.OrderByDescending(x => x.Id).FirstOrDefault();
            return MapBlog(query, preventIncrement);
        }

        private async Task<BlogDto> MapBlog(Blog inputBlog, bool preventIncrement)
        {
            var query = context.BlogItem.First(x => x.Id == inputBlog.Id);
            Blog blogTemp = query;

            if (!preventIncrement)
            {
                blogTemp.Requests++;
                await context.SaveChangesAsync();
            }

            BlogDto returnBlog = new();
            returnBlog.Id = blogTemp.Id;
            returnBlog.Title = blogTemp.Title;
            returnBlog.Content = blogTemp.Content;
            returnBlog.Requests = (int)blogTemp.Requests;
            returnBlog.DateCreated = blogTemp.DateCreated.ToString("yyyy/MM/dd");
            returnBlog.DateModified = blogTemp.DateModified.ToString("yyyy/MM/dd");

            return returnBlog;
        }

        public async Task<string> UploadImage([FromForm] IFormFile file)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            string path = Path.Combine(Directory.GetCurrentDirectory(), ImagesDirectory + timeStamp + Path.GetExtension(file.FileName));
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
