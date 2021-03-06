using API_BLOG.Contexts;
using API_BLOG.Models;
using API_BLOG.Models.BodyModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_BLOG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        PersonsContext PersonsDatabase;

        public PersonController(PersonsContext personsContext)
        {
            PersonsDatabase = personsContext;

        }

        #region UpdatePersonalData: ChangePassword, UpdatePersonalData
        // change password
        // update other data
        [Route("ChangePassword")]
        [HttpPut]
        [Authorize(Roles = "admin,user")]
        public IActionResult ChangePassword([FromBody] ChangePasswordData changePasswordData)
        {
            if (Request.Cookies.TryGetValue("ID", out string? uid) == false)
                return Unauthorized(new { error = "Cookie not found error. Authorize first" });

            var person = PersonsDatabase.Persons.Where(p => p.Id == Convert.ToInt32(uid)).FirstOrDefault();
            if (person == null)
                return BadRequest(new { error = $"Unknown error. Cannot find person with your cookie id: '{uid}'" });

            if (person.ComparePassword(changePasswordData.OldPassword) == false)
                return BadRequest(new { error = "old password was incorrect" });

            person.SetupNewPassword(changePasswordData.NewPassword);
            PersonsDatabase.Update(person);
            PersonsDatabase.SaveChanges();
            return Ok(new
            {
                message = "Your password sucessfully changed!",
                passwordHash = person.Password
            });
        }
        [Route("UpdatePersonalData")]
        [HttpPut]
        [Authorize(Roles = "admin,user")]
        public IActionResult UpdatePersonalData([FromBody] UpdateProfileData updateProfileData)
        {
            if (Request.Cookies.TryGetValue("ID", out string? uid) == false)
                return Unauthorized(new { error = "Cookie not found error. Authorize first" });

            var person = PersonsDatabase.Persons.Where(p => p.Id == Convert.ToInt32(uid)).FirstOrDefault();
            if (person == null)
                return BadRequest(new { error = $"Unknown error. Cannot find person with your cookie id: '{uid}'" });

            person.UpdatePersonalData(updateProfileData);
            PersonsDatabase.Update(person);
            PersonsDatabase.SaveChanges();

            return Ok(new
            {
                message = "Your personal data successfully updated",
                personalData = new
                {
                    email = person.Email,
                    country = person.Country,
                    profilePhoto = person.ProfilePhoto
                }
            });
        }
        #endregion

        //CreateArticle, ShowAllArticles
        [Route("CreateArticle")]
        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public IActionResult CreateArticle([FromBody] ArticleUpdate articleUpdateData)
        {
            // проверить куки
            // получить пользователя

            if (Request.Cookies.TryGetValue("ID", out string? uid) == false)
                return BadRequest(new { error = "Cookie not found error. Authorize first" });

            int _uid = Convert.ToInt32(uid);

            var person = PersonsDatabase.Persons.Where(p => p.Id == _uid).FirstOrDefault();
            if (person == null)
                return BadRequest(new { error = $"Unknown error. Cannot find person with your cookie id: '{uid}'" });

            if (articleUpdateData.Title == null || articleUpdateData.Content == null)
                return BadRequest(new { error = "Title and Content fields required to be" });

            var article = Article.Create(_uid, articleUpdateData);
            PersonsDatabase.Articles.Add(article);
            PersonsDatabase.SaveChanges();
            return Ok(new
            {
                message = "Article successfully added!",
                articleData = article
            });
        }
        [Route("GetArticlesForUser")]
        [HttpGet]
        [Authorize(Roles = "admin,user")]
        public IActionResult GetArticlesForUser()
        {
            // проверить куки -> взять id
            // проверить что пользователь с таким id есть
            // показать статьи

            if (!Request.Cookies.TryGetValue("ID", out string? uid))
                return BadRequest(new { error = "Cookie not found error. Authorize first" });

            var person = PersonsDatabase.Persons.Where(p => p.Id == Convert.ToInt32(uid)).FirstOrDefault();
            if (person == null)
                return BadRequest(new { error = $"Unknown error. Cannot find person with your cookie id: '{uid}'" });

            return Ok(new
            {
                message = "Request succeed",
                personArticles = PersonsDatabase.Articles.Where(a => a.PersonId == Convert.ToInt32(uid)).ToList()
            });
        }
    }
}