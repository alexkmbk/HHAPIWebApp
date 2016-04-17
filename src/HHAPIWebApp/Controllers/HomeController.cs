using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet;
using HHAPIWebApp.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;

namespace HHAPIWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _securityManager;

        public HomeController(UserManager<ApplicationUser> securityManager)
        {
            _securityManager = securityManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        // Возвращает HTML страницу со списком вакансий
        [Authorize]
        public IActionResult Vacancies(string searchString, bool openOnly=true)
        {
            HHApi hhapi = new HHApi();

            string token = null;
            string UserId = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = _securityManager.FindByEmailAsync(User.Identity.Name).Result;
                if (user != null && !string.IsNullOrEmpty(user.Token))
                {
                    token = user.Token;
                    UserId = user.UserId;
                }
            }
            // Получаем и передаем на страницу свойства пользователя
            try { 
            ViewData["UserInfo"] = hhapi.GetUserInfo(token, UserId);
            }
            // Если произошло исключение связанное с параметрами, отправляем юзера на страницу 
            // заполнения Token и UserId
            catch(Exception e) when (e is System.ArgumentException || e is AuthException)
            {
                return RedirectToAction(nameof(AccountController.Index), "Account");
            }
            // Получаем и передаем на страницу список вакансий
            ViewData["vacancies"] = hhapi.GetFavoriteVacancies(token, UserId, searchString, openOnly);
            ViewData["token"] = token;
            ViewData["UserId"] = UserId;
            return View();
        }

        // Возвращает HTML страницу с выводом свойств одной вакансии, перед этим запросив их на сервере HH.ru
        [HttpGet]
        public IActionResult Vacancy(string id)
        {
            HHApi hhapi = new HHApi();
            string token = null;
            string UserId = null;
            if (User.Identity.IsAuthenticated)
            {
                var user = _securityManager.FindByEmailAsync(User.Identity.Name).Result;
                if (user != null && !string.IsNullOrEmpty(user.Token))
                {
                    token = user.Token;
                    UserId = user.UserId;
                }
            }
            ViewData["VacancyInfo"] = hhapi.GetVacancyInfo(token, UserId, id);
            
             return View();
        }

    }
}
