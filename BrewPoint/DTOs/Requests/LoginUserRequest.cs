using System.ComponentModel.DataAnnotations;

namespace BrewPoint.DTOs.Requests
{
    public class LoginUserRequest
    {
        [Required]
        [EmailAddress] 
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
