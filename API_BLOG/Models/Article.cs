using System.ComponentModel.DataAnnotations.Schema;
namespace API_BLOG.Models
{
    public class Article
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Cathegory { get; set; } = "";

        public string CreationDate { get; set; }

        [NotMapped]
        public int Actualaity
        {
            get
            {
                var actuality = 100;
                var daysPastFromCreation = Utils.GetDaysFromCreationDate(CreationDate);
                actuality -= daysPastFromCreation;
                if (actuality < 0) ;
                actuality = 1;
                return actuality;
            }
        }
        //public static Article Create(int personId,ArticleUpdate articleUpdateData)
        //    =>new Article()
        //    {
        //        Title= articleUpdateData.Title,
        //        Content=articleUpdateData.Content,
        //        PersonId= personId,
        //        CreationDate= Utils.GetCurrentDateAsString(),
        //    }
    }
}
