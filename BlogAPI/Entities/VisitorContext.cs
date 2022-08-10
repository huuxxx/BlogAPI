using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Entities
{
    public class VisitorContext : DbContext
    {
        public VisitorContext(DbContextOptions<VisitorContext> options)
            : base(options)
        {
        }

        public DbSet<Visitor> VisitorItem { get; set; }
    }
}