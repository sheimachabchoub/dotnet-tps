using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Shadow foreign key: the shared Employee model only has the navigation property
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department).WithMany().HasForeignKey("DepartmentId");

            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentId = 1, DepartmentName = "IT" },
                new Department { DepartmentId = 2, DepartmentName = "HR" },
                new Department { DepartmentId = 3, DepartmentName = "Payroll" },
                new Department { DepartmentId = 4, DepartmentName = "Admin" });

            modelBuilder.Entity<Employee>().HasData(
                new { EmployeeId = 1, FirstName = "John", LastName = "Hastings", Email = "john@pragimtech.com",
                      DateOfBrith = new DateTime(1980, 10, 5), Gender = Gender.Male, DepartmentId = 1, PhotoPath = "images/john.svg" },
                new { EmployeeId = 2, FirstName = "Sam", LastName = "Galloway", Email = "sam@pragimtech.com",
                      DateOfBrith = new DateTime(1981, 12, 22), Gender = Gender.Male, DepartmentId = 2, PhotoPath = "images/sam.svg" },
                new { EmployeeId = 3, FirstName = "Mary", LastName = "Smith", Email = "mary@pragimtech.com",
                      DateOfBrith = new DateTime(1979, 11, 11), Gender = Gender.Female, DepartmentId = 1, PhotoPath = "images/mary.svg" },
                new { EmployeeId = 4, FirstName = "Sara", LastName = "Longway", Email = "sara@pragimtech.com",
                      DateOfBrith = new DateTime(1982, 9, 23), Gender = Gender.Female, DepartmentId = 3, PhotoPath = "images/sara.svg" });
        }
    }
}
