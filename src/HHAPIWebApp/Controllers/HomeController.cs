﻿using System;
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
            ViewData["UserInfo"] = HHApi.GetUserInfo(token, UserId);
            }
            catch(System.ArgumentException e)
            {
                return RedirectToAction(nameof(AccountController.Index), "Account");
            }
            // Получаем и передаем на страницу список вакансий
            ViewData["vacancies"] = HHApi.GetFavoriteVacancies(token, UserId, searchString, openOnly);
            return View();
        }

        // Возвращает HTML страницу с выводом свойств одной вакансии, перед этим запросив их на сервере HH.ru
        [HttpGet]
        public IActionResult Vacancy(string id)
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
            ViewData["VacancyInfo"] = HHApi.GetVacancyInfo(token, UserId, id);
            
             return View();
        }

    }
}
