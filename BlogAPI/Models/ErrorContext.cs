using BlogAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Models
{
    public class ErrorContext : DbContext
    {
        public ErrorContext(DbContextOptions<ErrorContext> options)
            : base(options)
        {
        }

        public DbSet<ErrorItem> ErrorItem { get; set; }
    }
}