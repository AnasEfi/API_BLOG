using API_BLOG.Models;
using Microsoft.EntityFrameworkCore;

namespace API_BLOG.Contexts
{
    public class PersonsContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Achievment> Achievments { get; set; }
        public PersonsContext(DbContextOptions<PersonsContext> opts) : base(opts)

        {
            Database.EnsureCreated();

            //    if (Persons != null)
            //    {
            //        foreach (var p in Persons)
            //        {
            //            if (p.Achievments != null && p.Achievments.Length > 0)
            //                p.UpdateAchivmentsListFromJson();
            //        }
            //    }
            //}




        }
    }
}
