using EmployeeManagementAPI.Models;
using Microsoft.AspNetCore.Identity;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace EmployeeManagementAPI.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    // DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Database=TestDB;User=rooot;Password=0193247637mM!;",
                    new MySqlServerVersion(new Version(8, 0, 4))); // Adjust based on your MySQL version
            }
        }

    }
}
