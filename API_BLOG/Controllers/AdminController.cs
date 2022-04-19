using API_BLOG.Contexts;
using API_BLOG.Models;
using API_BLOG.Models.BodyModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API_BLOG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        PersonsContext PersonsDatabase { get; set; }

        public AdminController(PersonsContext persContext)
        {
            PersonsDatabase = persContext;

        }

        #region Delete/Update Person
        [Route("DeletePerson/{id}")]
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeletePerson(int Id)
        {
            var person = PersonsDatabase.Persons.Where(u => u.Id == Id).FirstOrDefault();
            if (person == null)
                return NotFound(new { messege = "User is not found" });
            PersonsDatabase.Persons.Remove(person);
            PersonsDatabase.SaveChanges();
            return Ok(new { messege = $"User with id {Id} is deleted" });
        }

        [Route("UpdatePerson")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdatePerson([FromBody] PersonUpdate personUpdate)
        {
            var person = PersonsDatabase.Persons.Where(u => u.Id == personUpdate.Id).FirstOrDefault();
            if (person == null)
                return NotFound(new { messege = "User is not found" });

            person.Email = personUpdate.Email ?? person.Email;
            person.IsAdmin = personUpdate.IsAdmin ?? person.IsAdmin;
            person.Country = personUpdate.Country ?? person.Country;
            person.ProfilePhoto = personUpdate.ProfilePhoto ?? person.ProfilePhoto;
            if (personUpdate.Achievments != null)
            {
                person.Achievments = personUpdate.Achievments;
                person.UpdateAchivmentsListFromJson();
            }

            PersonsDatabase.Update(person);
            PersonsDatabase.SaveChanges();
            return Ok(new { messege = $"User with id {personUpdate.Id} is updated" });
        }
        #endregion

        #region Update/Delete Article & GiveArticleCategory

        [Route("UpdateArticle/{id}")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateArticle(int id, [FromBody] ArticleUpdate articleData)
        {
            // Получаем id, получаем данные для изменения
            // Пытаемся найти Article с таким id
            // => Если не нашли -> NotFound
            // => Если нашли -> Изменяем его в бд

            var article = PersonsDatabase.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article == null)
                return NotFound(new { error = $"Article with id {id} not found" });

            article.Title = articleData.Title ?? article.Title;
            article.Category = articleData.Category ?? article.Category;
            article.Content = articleData.Content ?? article.Content;

            PersonsDatabase.Update(article);
            PersonsDatabase.SaveChanges();
            return Ok(new { message = $"Article's data with id '{id}' successfully updated" });
        }

        [Route("DeleteArticle/{id}")]
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteArticle(int id)
        {
            var article = PersonsDatabase.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article == null)
                return NotFound(new { messege = "Article is not found" });

            PersonsDatabase.Articles.Remove(article);
            PersonsDatabase.SaveChanges();

            return Ok(new { messege = $"Article with id '{id}' is deleted" });
        }

        [Route("GiveCategoryToArticle/{id}")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult GiveCategoryToArticle(int id, [FromBody] Category category)
        {
            // проверяем есть article с таким id
            // обновляем категорию
            var article = PersonsDatabase.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article == null)
                return NotFound(new { error = $"Article with id '{id}' doesn't exists" });

            article.Category = category.Name;
            PersonsDatabase.Update(article);
            PersonsDatabase.SaveChanges();

            return Ok(new { message = $"Article with id '{id}' now has name '{category.Name}'" });
        }

        #endregion

        #region Create/Update/Delete and GivePerson Achievment
        [Route("GivePersonAnAchievment/{id}")] // person id
        [HttpPut]
        [Authorize(Roles = "admin")]
        // id - person id
        public IActionResult GivePersonAnAchievment(int id, [FromBody] AchievmentData achievmentData)
        {
            if (achievmentData.Id == null)
                return BadRequest(new { error = "Achievment Id field is required for method GivePersonAnAchievment" });
            // проверить существования пользователя с таким id
            // проверить существование ачивки с таким id
            // проверить, что у пользователя уже нет такой ачивки
            // добавить ачивку пользователю
            var person = PersonsDatabase.Persons.Where(p => p.Id == id).FirstOrDefault();
            if (person == null)
                return NotFound(new { error = $"Person with id '{id}' not found" });

            var achievment = PersonsDatabase.Achievments.Where(a => a.Id == achievmentData.Id).FirstOrDefault();
            if (achievment == null)
                return NotFound(new { error = $"Achievment with id '{achievmentData.Id}' not found" });

            if (person.TryAddNewAchievment(achievment) == false)
                return BadRequest(new { error = $"Person with id '{person.Id}' already have an achievment with id '{achievment.Id}'" });

            person.UpdateAchievmentsJsonFromList();
            PersonsDatabase.SaveChanges();
            return Ok(new
            {
                message = $"Achievment with id '{achievment.Id}' successfully added to person with id '{person.Id}'",
                personData = person
            });
        }
        [Route("CreateAchievment")]
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult CreateAchievment([FromBody] AchievmentData achievmentData)
        {
            if (achievmentData.Name == null)
                return BadRequest(new { error = "You cannot create achievment w/o field Name" });

            // Проверим есть ли уже в БД ачивка с таким именем
            var achievment = PersonsDatabase.Achievments.Where(a => a.Name.ToLower().Equals(achievmentData.Name.ToLower())).FirstOrDefault();
            if (achievment != null)
                return BadRequest(new { error = $"Achievment with name '{achievment.Name}' already exists" });

            achievment = Achievment.Create(achievmentData.Name);
            PersonsDatabase.Achievments.Add(achievment);
            PersonsDatabase.SaveChanges();
            return Ok(new
            {
                message = "Achievment successfully created.",
                achievmentData = achievment
            });
        }
        [Route("UpdateAchievment")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateAchievment([FromBody] AchievmentData achievmentData)
        {
            if (achievmentData.Id == null)
                return BadRequest(new { error = "Field 'Id' required to UpdateAchievment method" });

            // проверить есть ли такая ачивка по id
            // заменить полученные поля
            // обновить ачиыку
            var achievment = PersonsDatabase.Achievments.Where(a => a.Id == achievmentData.Id).FirstOrDefault();
            if (achievment == null)
                return NotFound(new { error = $"Achievment with id '{achievmentData.Id}' not found!" });

            achievment.Name = achievmentData.Name ?? achievment.Name;

            PersonsDatabase.Achievments.Add(achievment);
            PersonsDatabase.SaveChanges();

            return Ok(new
            {
                message = "Achievment successfully updated.",
                achievmentData = achievment
            });
        }
        [Route("DeleteAchievment/{id}")]
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteAchievment(int id)
        {
            // проверить наличие ачивки по id
            // удалить ачивку
            var achievment = PersonsDatabase.Achievments.Where(a => a.Id == id).FirstOrDefault();
            if (achievment == null)
                return NotFound(new { error = $"Achievment with id '{id}' not found" });

            foreach (var p in PersonsDatabase.Persons)
            {
                if (p.AchievmentsList.Contains(id))
                {
                    p.AchievmentsList.Remove(id);
                    p.UpdateAchievmentsJsonFromList();
                }
            }
            PersonsDatabase.SaveChanges();

            PersonsDatabase.Achievments.Remove(achievment);
            PersonsDatabase.SaveChanges();

            return Ok(new { message = $"Ahievment with id '{id}' successfully deleted" });
        }

        #endregion

        #region GetAllArticles
        [Route("GetAllAchievments")]
        [HttpGet]
        [Authorize(Roles = "admin")]

        public IActionResult GetAllAchievments()
            => Ok(new { achievments = PersonsDatabase.Achievments });

        #endregion

        [Authorize(Roles = "admin")]
        [Route("ShowAllPersonsData")]
        [HttpGet]
        public IActionResult ShowAllPersonsData()
            => Ok(new
            {
                message = "All persons data",
                data = PersonsDatabase.Persons.Select(p => new
                {
                    email = p.Email,
                    password = p.Password,
                    isAdmin = p.IsAdmin,
                    registrationDate = p.RegistrationDate,
                    achievments = p.Achievments,
                    country = p.Country,
                    photo = p.ProfilePhoto
                }).ToList()
            });
    }
}
