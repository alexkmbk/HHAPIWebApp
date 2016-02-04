using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using System.Security.Cryptography;

using HHAPIWebApp.Models;

namespace HHAPIWebApp.Database
{
    public class SampleDataInitializer
    {
        private ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;
        //private RoleManager<AppRole> _roleManager;

        public SampleDataInitializer(ApplicationDbContext ctx, UserManager<ApplicationUser> UserManager)
        {
            _ctx = ctx;
            _userManager = UserManager;
            //_roleManager = RoleManager;

            //var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

        }

        public async void InitializeData()
        {
            ApplicationUser foundUser;
            try { 
            foundUser = _userManager.FindByEmailAsync("admin@foo.com").Result;
            }
            catch (Exception e)
            {
                return;
            }
            if (foundUser == null)
            {
               
                var user = new ApplicationUser { UserName = "admin", Email = "admin@foo.com" };
                await _userManager.CreateAsync(user);
                await _userManager.AddPasswordAsync(user, "123");
                foundUser = user;
            }
            else
            {
                //_userManager.ChangePasswordAsync(foundUser, "", HashPassword("123"));
            }
            /*var foundRole = await _roleManager.FindByNameAsync("Administrator");
            if (foundRole == null)
            {

                AppRole role = new AppRole();
                role.Name = "Administrator";
                var res = await _roleManager.CreateAsync(role);
                _ctx.SaveChanges();
                res = await _userManager.AddToRoleAsync(foundUser, "Administrator");
            }*/
        }
    }
}
