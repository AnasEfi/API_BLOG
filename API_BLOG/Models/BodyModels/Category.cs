using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Models.BodyModels
{
    public class Category
    {
        [Required]
        public string Name { get; set; }
    }
}
