using Microsoft.EntityFrameworkCore;

namespace SaigonRideSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet sẽ thêm sau khi mình có tài liệu Class Diagram/ERD
        // Ví dụ: public DbSet<Station> Stations { get; set; }
    }
}