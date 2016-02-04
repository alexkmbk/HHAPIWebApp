using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace HHAPIWebApp.Models
{
    public class ApplicationUser: IdentityUser    {
        public string Token { get; set; }
        public string UserId { get; set; }
    }

    public class AppRole : IdentityRole
    {
        
        
    }
 
}
