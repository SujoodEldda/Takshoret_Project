using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CofeeShop.Models
{
    public class User
    {
        [Key]
        [Required]
        public string userName { get; set; }
        public string password { get; set; }
        public int age { get; set; }
        public string userType { get; set; }
        public int CountCofee { get; set; }

    }
}