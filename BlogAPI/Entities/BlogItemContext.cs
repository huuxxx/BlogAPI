using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Entities
{
    public class BlogItemContext : DbContext
    {
        public BlogItemContext(DbContextOptions<BlogItemContext> options)
            : base(options)
        {
        }

        public DbSet<BlogItem> BlogItem { get; set; }
        public object BlogItemDTO { get; internal set; }
    }
}