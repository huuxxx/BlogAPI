using BlogAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Models
{
    public class VisitorContext : DbContext
    {
        public VisitorContext(DbContextOptions<VisitorContext> options)
            : base(options)
        {
        }

        public DbSet<VisitorItem> VisitorItem { get; set; }
        public object VisitorItemDTO { get; internal set; }
    }
}