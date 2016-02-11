using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace HHAPIWebApp.Models
{
    public class Register
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        
        [Display(Name = "HH.ru token")]
        public string Token { get; set; }

        [Display(Name = "user id")]
        public string UserId { get; set; }

   }
}
