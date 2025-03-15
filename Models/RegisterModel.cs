using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.Models
{
    public class RegisterModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin or User
    }
}
