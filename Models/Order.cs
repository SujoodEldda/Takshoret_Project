using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CofeeShop.Models
{
    public class Order
    {
        [Key]
        public int orderId { get; set; }
        public int itemId { get; set; }
        public int tableId { get; set; }
        public string userName { get; set; }
        public bool isPayed { get; set; }
        public bool isReady { get; set; }
    }
}