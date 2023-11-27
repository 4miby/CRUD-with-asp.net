using Assignment5.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Assignment5.Controllers
{
    public class MVCDemoDbContextcs : DbContext
    {
        public MVCDemoDbContextcs(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
