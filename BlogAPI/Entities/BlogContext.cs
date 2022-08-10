using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Entities
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
        }

        public DbSet<BlogItem> BlogItem { get; set; }
    }
}