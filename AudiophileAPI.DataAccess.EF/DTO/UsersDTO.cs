using System.ComponentModel.DataAnnotations;

namespace AudiophileAPI.DTO
{
    public class UsersDTO
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
