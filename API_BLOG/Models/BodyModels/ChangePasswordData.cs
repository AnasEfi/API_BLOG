using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Models.BodyModels
{
    public class ChangePasswordData
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
