namespace API_BLOG.Models
{
    public class Achievment
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string CreationDate { get; set; }
        public static Achievment Create(string name)
        {
            return new Achievment() { Name = name, CreationDate = Utils.GetCurrentDateAsString() }; 
        }

    }
}
