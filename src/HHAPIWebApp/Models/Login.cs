using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;


namespace HHAPIWebApp.Models
{
    public class Login
    {
        [Required]
        //[EmailAddress]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

   
    }
}
