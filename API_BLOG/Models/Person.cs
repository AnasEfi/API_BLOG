
using System.ComponentModel.DataAnnotations.Schema;

namespace API_BLOG.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string Country { get; set; }
        public string ProfilePhoto { get; set; }
        public string Achievements { get; set; }

        [NotMapped]
        public List<int> AchievmentsList { get; set; } = new List<int>();

        [NotMapped]
        public static readonly string DefaultUserPhoto = "default_photo.jfif";
    }
}
