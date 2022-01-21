using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CofeeShop.Models;
using CofeeShop.DAL;
using CofeeShop.Controllers;

namespace CofeeShop.ViewModel
{
    public class TableViewModel
    {
        public Table table { get; set; }
        public List<Table> tables { get; set; }
    }
}