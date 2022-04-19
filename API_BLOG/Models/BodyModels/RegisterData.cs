using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Models.BodyModels
{
    public class RegisterData
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Country { get; set; }
        public string? ProfilePhoto { get; set; }

    }
}