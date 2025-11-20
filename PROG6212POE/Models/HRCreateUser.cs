using System.ComponentModel.DataAnnotations;

namespace PROG6212POE.Models
{
    public class HRCreateUser
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
