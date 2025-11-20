using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class Login
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // ADDED FOR HR FUNCTIONALITY
        public string Role { get; set; } // Lecturer, Coordinator, Manager, HR

        // Lecturer-only fields
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public decimal? HourlyRate { get; set; }
    }
}
