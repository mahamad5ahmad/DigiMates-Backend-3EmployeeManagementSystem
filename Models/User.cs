using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed password

        [Required]
        public string Role { get; set; } = "User"; // Default role
    }
}
