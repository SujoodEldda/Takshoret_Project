using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace CofeeShop.Models
{
    public class Item
    {
        [Key]
        public int itemId { get; set; }
        public string name { get; set; }
        public bool isAvailable { get; set; }
        public double price { get; set; }
        public double prePrice { get; set; }
        public string category { get; set; }
        public int age { get; set; }
        public string ImageUrl { get; set; }
    }
}