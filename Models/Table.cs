using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CofeeShop.Models
{
    public class Table
    {
        [Key]
        public int tableId { get; set; }
        public int seatsNumber { get; set; }
        public string status { get; set; }
        public int n { get; set; }
    }
}