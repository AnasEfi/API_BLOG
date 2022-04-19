using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Models.BodyModels
{
    public class SearchOptions
    {
        [Required]
        public string Keywords { get; set; }
    }
}
