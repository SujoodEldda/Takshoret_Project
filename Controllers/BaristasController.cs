using CofeeShop.DAL;
using CofeeShop.Models;
using CofeeShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CofeeShop.Controllers;


namespace CofeeShop.Controllers
{
    public class BaristasController : Controller
    {
        // GET: Baristas
        public ActionResult ShowAllOrders()
        {
            Dal dal = new Dal();
            OrderViewModel cvm = new OrderViewModel();
            List<Order> orders = (from x in dal.orders
                                  where x.isReady==false
                                  select x).ToList<Order>();
            List<Item> items = (from x in dal.items
                                select x).ToList<Item>();
            List<Order> z = new List<Order>();
            foreach (Order o in orders)
            {
                foreach (Item i in items)
                {
                    if (i.itemId == o.itemId)
                        z.Add(o);
                }
            }
            List<Order> last = z.Distinct().ToList();
            //remove orders with the items that have been removed
            cvm.order = new Order();
            cvm.orders = last;
            return View(cvm);
        }
        
            public ActionResult ChangOrder()
        {
            Dal dal = new Dal();
            OrderViewModel cvm = new OrderViewModel();
            List<Order> orders = (from x in dal.orders
                                  where x.isReady == false
                                  select x).ToList<Order>();
            cvm.order = new Order();
            cvm.orders = orders;
            return View(cvm);
        }
        public ActionResult ChangeOrder()
        {
            Dal dal = new Dal();
            int id;
            Int32.TryParse(Request.Form["ChangeOrder"], out id);
            Session["OrderId"] = id;
            return View();

        }
        
            public ActionResult Replace()
        {
            Dal dal = new Dal();

            int xx = (int)Session["OrderId"];
            string choice = (string)Request.Form["namee"];
            List<Order> orders = (from x in dal.orders
                                  where x.isReady == false&&x.isPayed==false&&x.orderId== xx
                                  select x).ToList<Order>();
           
            if (some(choice))
            {
                TempData["msg"] = "there is no such an item, enter an register item !!";
                TempData["color"] = "red";
                return View("ChangeOrder");
            }
            
            
            if (some1(xx))
            {
                TempData["msg"] = "this item is already ready or you have paid for it !!";
                TempData["color"] = "red";
                return View("ChangeOrder");
            }
            List<Item> it = (from x in dal.items
                             where x.name == choice
                             select x).ToList<Item>();
            orders[0].itemId = it[0].itemId;
            dal.SaveChanges();
            TempData["msg"] = "Changed successfully !!";
            TempData["color"] = "blue";
            return View("ChangeOrder");

        }
        public bool some(string ch)
        {
            Dal dal = new Dal();
            List<Item> it = (from x in dal.items
                             where x.name == ch
                             select x).ToList<Item>();
            if (it.Count==0)
                return true;
            return false;
        }
        public bool some1(int xx)
        {
            Dal dal = new Dal();
            List<Order> orders = (from x in dal.orders
                                  where x.isReady == false && x.isPayed == false && x.orderId == xx
                                  select x).ToList<Order>();

            if (orders.Count == 0)
                return true;
            return false;
        }
        
            public ActionResult ChangeTablePlace()
        {
            int xx = (int)Session["OrderId"];
            string choice = (string)Request.Form["statuss"];
            Dal dal = new Dal();
            List<Order> cc = (from x in dal.orders
                              where x.orderId == xx
                              select x).ToList<Order>();
            int z = cc[0].tableId;
            if (some2(z))
            {
                TempData["msg"] = "there is no table have been booked to this order !!";
                TempData["color"] = "red";
                return View("ChangeOrder");
            }
           
            List<Table> zz = (from x in dal.tables
                              where x.tableId == z
                              select x).ToList<Table>();
            if (zz[0].status==choice)
            {
                TempData["msg"] = "nothing to change !!";
                TempData["color"] = "red";
                return View("ChangeOrder");
            }
            
            zz[0].status = choice;
            dal.SaveChanges();
            TempData["msg"] = "changed successfully !!";
            TempData["color"] = "blue";
            return View("ChangeOrder");
        }
        public bool some2(int z)
        {
            Dal dal = new Dal();
            List<Table> zz = (from x in dal.tables
                              where x.tableId == z
                              select x).ToList<Table>();
            if (zz.Count == 0)
                return true;
            return false;
        }
        
            public ActionResult TakePayment()
        {
            Dal dal = new Dal();
            OrderViewModel cvm = new OrderViewModel();
            List<Item> items = (from x in dal.items
                                  select x).ToList<Item>();
            List<Order> orders = (from x in dal.orders
                                  where x.isPayed == false
                                  select x).ToList<Order>();
            List<Order> z=new List<Order>();
            foreach (Order o in orders){
                foreach (Item i in items)
                {
                    if (i.itemId == o.itemId)
                        z.Add(o);
                }
            }
            List<Order> last = z.Distinct().ToList();
            //remove orders with the items that have been removed
            cvm.order = new Order();
            cvm.orders = last;
            return View(cvm);
        }
        
            public ActionResult Payed()
        {
            int xz;
            Int32.TryParse(Request.Form["Payedd"], out xz); 
            Dal dal = new Dal();
            List<Order> cc = (from x in dal.orders
                              where x.orderId == xz
                              select x).ToList<Order>();
           
            cc[0].isPayed = true;
            dal.SaveChanges();
            TempData["msg"] = "Payed successfully !!";
            TempData["color"] = "blue";
            return RedirectToAction("TakePayment", "Baristas");
        }
        public ActionResult Redy()
        {
            int xz;
            Int32.TryParse(Request.Form["Redyy"], out xz);
            Dal dal = new Dal();
            List<Order> cc = (from x in dal.orders
                              where x.orderId == xz
                              select x).ToList<Order>();

            cc[0].isReady = true;
            dal.SaveChanges();
            TempData["msg"] = "the order is ready !!";
            TempData["color"] = "blue";
            return RedirectToAction("ShowAllOrders", "Baristas");
        }
        public ActionResult ChangeTableStatus()
        {
            return View();
        }
        public ActionResult ChangeStatus(Table obj)
        {
            int xz = obj.n;
            Dal dal = new Dal();
            

            if (some6(xz))
            {
                TempData["msg"] = "this table is not in the list !!";
                TempData["color"] = "red";
                return RedirectToAction("ChangeTableStatus", "Baristas");
            }
            List<Order> cc = (from x in dal.orders
                              where x.tableId == xz
                              select x).ToList<Order>();

            if (obj.status == "out")
            {
                TempData["msg"] = "this table is already occupted !!";
                TempData["color"] = "red";
                return RedirectToAction("ChangeTableStatus", "Baristas");

            }
            cc[0].tableId = 0;
            dal.SaveChanges();
            TempData["msg"] = "changed successfully !!";
            TempData["color"] = "blue";
            return RedirectToAction("ChangeTableStatus", "Baristas");
        }
        public bool some6(int z)
        {
            Dal dal = new Dal(); 
            List<Order> ccc = (from x in dal.orders
                                                   where x.tableId == z
                                                   select x).ToList<Order>();
            
            if (ccc.Count == 0)
                return true;
            return false;
        }
    }
}