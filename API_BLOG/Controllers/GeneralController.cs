using API_BLOG.Contexts;
using API_BLOG.Models;
using API_BLOG.Models.BodyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        // show all articles
        // show all users data

        PersonsContext PersonsDatabase { get; set; }
        public GeneralController(PersonsContext personsContext)
        {
            PersonsDatabase = personsContext;
        }
        [Route("ShowAllPersonsData")]
        [HttpGet]
        public IActionResult ShowAllPersonsData()
           => Ok(new
           {
               message = "All persons data",
               data = PersonsDatabase.Persons.Where(p => p.IsAdmin == false).Select(p => new
               {
                   email = p.Email,
                   isAdmin = p.IsAdmin,
                   registrationDate = p.RegistrationDate,
                   achievments = p.Achievments,
                   country = p.Country,
                   photo = p.ProfilePhoto
               }).ToList()
           });

        [Route("ShowAllArticles")]
        [HttpGet]
        public IActionResult ShowAllArticles()
            => Ok(new
            {
                message = "All articles",
                articles = PersonsDatabase.Articles
            });

        [Route("SearchArticles")]
        [HttpPost]
        public IActionResult SearchArticles([FromBody] SearchOptions options)
            => Ok(new
            {
                articles = PersonsDatabase.Articles.Where(a => a.Title.ToLower().Contains(options.Keywords.ToLower())).ToList()
            });

        [Route("ShowActualArticles/{actuality}")]
        [HttpGet]
        public IActionResult ShowActualArticles(int actuality)
        {
            var actualArticles = new List<Article>();
            foreach (var a in PersonsDatabase.Articles)
            {
                if (a.Actuality >= actuality)
                    actualArticles.Add(a);
            }

            return Ok(new
            {
                message = "Actual Articles",
                articles = actualArticles
            });
        }
    }
}

