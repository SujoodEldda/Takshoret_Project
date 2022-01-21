using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CofeeShop.Models
{
    public class Today
    {
        [Key]
        public int todayId { get; set; }
        public string dishoftheday { get; set; }
        public bool isParty { get; set; }
        public bool isRainy { get; set; }
    }
}