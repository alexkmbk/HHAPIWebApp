using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using System.Security.Cryptography;

using HHAPIWebApp.Models;

namespace HHAPIWebApp.Database
{
    // Класс для первичного заполнения базы данных
    public class SampleDataInitializer
    {
        private ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;
  
        public SampleDataInitializer(ApplicationDbContext ctx, UserManager<ApplicationUser> UserManager)
        {
            _ctx = ctx;
            _userManager = UserManager;
        }

        public async void InitializeData()
        {
            ApplicationUser foundUser;
            // Поиск пользователя по email
            try { 
            foundUser = _userManager.FindByEmailAsync("admin@foo.com").Result;
            }
            catch 
            {
                return;
            }
            // если пользователь не найден, создаем нового
            if (foundUser == null)
            {
               
                var user = new ApplicationUser { UserName = "admin", Email = "admin@foo.com" };
                await _userManager.CreateAsync(user);
                // здесь не правильно передается пароль, нужно похоже передавать хэш пароля
                await _userManager.AddPasswordAsync(user, "123");
                foundUser = user;
            }
        }
    }
}
