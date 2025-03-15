using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public decimal Salary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}