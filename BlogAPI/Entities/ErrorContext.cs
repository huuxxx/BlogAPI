using BlogAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Entities
{
    public class ErrorContext : DbContext
    {
        public ErrorContext(DbContextOptions<ErrorContext> options)
            : base(options)
        {
        }

        public DbSet<ErrorItemDTO> ErrorItem { get; set; }
    }
}