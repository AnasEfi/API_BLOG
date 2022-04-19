using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Models.BodyModels
{
    public class AuthorizationData
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
