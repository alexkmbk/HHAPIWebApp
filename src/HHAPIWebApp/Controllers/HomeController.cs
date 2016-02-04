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

        private List<Vacancy> _vacancies;

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

        [Authorize]
        public IActionResult Vacancies(string searchString, bool openOnly=true)
        {
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
            ViewData["UserInfo"] = HHApi.GetUserInfo(token, UserId);

            _vacancies = HHApi.GetFavoriteVacancies(token, UserId, searchString, openOnly);
            ViewData["vacancies"] = _vacancies;
            return View();
        }

        public IActionResult Vacancy(string id, string name, string url, string raw_adress, string AreaName)
        {
            //var vacancy = _vacancies.Find(x => x.id == id);
            Vacancy vacancy = new Vacancy();
            vacancy.id = id;
            vacancy.name = name;
            vacancy.url = url;
            vacancy.raw_adress = raw_adress;
            vacancy.AreaName = AreaName;
            return View(vacancy);
        }

    }
}
