using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CofeeShop.Models;
using CofeeShop.DAL;


namespace CofeeShop.ViewModel
{
    public class OrderViewModel
    {
        public Order order { get; set; }
        public List<Order> orders { get; set; }
        public double GetPrice(int id)
        {
            Dal dal = new Dal();
            List<Item> items = (from x in dal.items
                                where x.itemId.Equals(id)
                                  select x).ToList<Item>();
            return items[0].price;
        }
        public bool isAllPayed()
        {
            var rsult = (from x in orders where x.isPayed == false select x).ToList<Order>();
            return rsult.Count == 0;
        }
        public string GetName(int id)
        {
            Dal dal = new Dal();
            List<Item> items = (from x in dal.items
                                  where x.itemId.Equals(id)
                                  select x).ToList<Item>();
            return items[0].name;
        }
        public bool isAllReady(int id)
        {
            var rsult = (from x in orders where x.isReady == false select x).ToList<Order>();
            return rsult.Count == 0;
        }
    }
}