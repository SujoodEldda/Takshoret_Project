using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CofeeShop.DAL;
using CofeeShop.ViewModel;
using CofeeShop.Models;
using System.IO;
using CofeeShop.Controllers;

namespace CofeeShop.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult MangmentItems()
        {
            Dal dal = new Dal();
            ItemViewModel cvm = new ItemViewModel();
            List<Item> items = dal.items.ToList<Item>();
            cvm.item = new Item();
            cvm.items = items;
            return View(cvm);
        }
        public ActionResult UpdatePrice()
        {
            int ItemId;
            double pricee=6;
            Int32.TryParse(Request.Form["UpdatePrice"], out ItemId);
            string ava = (string)Request.Form["NewPrice"];
            Double.TryParse(ava, out pricee);
            Dal dal = new Dal();
            List<Item> items = (from x in dal.items
                                where x.itemId.Equals(ItemId)
                                  select x).ToList<Item>();
            items[0].prePrice = items[0].price;
            items[0].price = pricee;
            dal.SaveChanges();
            return RedirectToAction("MangmentItems", "Admin");
        }
        
            public ActionResult UpdateAvailable()
        {
            int ItemId;
            Int32.TryParse(Request.Form["UpdateAvailable"], out ItemId);
            string ava = (string)Request.Form["Available"];
            Dal dal = new Dal();
            List<Item> items = (from x in dal.items
                                where x.itemId.Equals(ItemId)
                                select x).ToList<Item>();
            if (ava.Contains("yes"))
                items[0].isAvailable = true;
            else
                items[0].isAvailable = false;
            dal.SaveChanges();
            return RedirectToAction("MangmentItems", "Admin");
        }
        public ActionResult RemoveItem()
        {
            Dal dal = new Dal();
            int id;
            Int32.TryParse(Request.Form["RemoveItem"], out id);
            List<Item> exist = (from x in dal.items where x.itemId.Equals(id) select x).ToList<Item>();

            var ordersNumber = (from x in dal.orders where x.itemId == id && x.isReady==false
                                select x).ToList<Order>();

            if (ordersNumber.Count == 0)
            {

                dal.items.Remove(exist[0]);
                dal.SaveChanges();
                return RedirectToAction("MangmentItems", "Admin");
            }
            TempData["msg"] = "Can't delete this item , The User Already take order";
            TempData["color"] = "red";
            return RedirectToAction("MangmentItems");

        }
        public ActionResult AddItem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddNewItem(Item obj, Image img)
        {
            string id = obj.category;
            Dal dal = new Dal();
            obj.isAvailable = true;
            
            string filename = Path.GetFileNameWithoutExtension(img.image.FileName);
            string extension = Path.GetExtension(img.image.FileName);
            filename = filename + obj.itemId + extension;
            obj.ImageUrl = "~/Images/" + filename;
            filename = Path.Combine(Server.MapPath("~/Images/"), filename);
            img.image.SaveAs(filename);
            dal.items.Add(obj);
            dal.SaveChanges();
            List<Item> existt = (from x in dal.items where x.name.Equals(obj.name) select x).ToList<Item>();
            dal.SaveChanges();

            TempData["msg"] = "Item added successfully !!";
            TempData["color"] = "blue";
            return View("AddItem");
        }
        public ActionResult ManagingTables()
        {
            return View();
        }
        public ActionResult AddTable(Table obj)
        {
            Dal dal = new Dal();
            List<Table> exist = (from x in dal.tables where x.n.Equals(obj.n) select x).ToList<Table>();

            if (exist.Count != 0)
            {
                TempData["msg"] = "Table exist !!!";
                TempData["color"] = "red";
                return View("ManagingTables");
            }
            

            dal.tables.Add(obj);
            dal.SaveChanges();
            TempData["msg"] = "table added successfully !!!";
            TempData["color"] = "blue";
            return View("ManagingTables");
        }
        public ActionResult UpdateTableSeatsAndPlace(Table obj)
        {
            Dal dal = new Dal();
            List<Table> exist = (from x in dal.tables where x.n.Equals(obj.n) select x).ToList<Table>();
            if (exist.Count == 0)
            {
                TempData["msg"] = "Table is not exist !!";
                TempData["color"] = "red";
                return View("ManagingTables");
            }
            exist[0].seatsNumber = obj.seatsNumber;
            exist[0].status = obj.status;
            dal.SaveChanges();
            TempData["msg"] = "changed successfully !!";
            TempData["color"] = "blue";
            return View("ManagingTables");
        }
            public ActionResult AddItemOfDay(Table obj)
        {
            return View();
        }
        
            public ActionResult AddNewItemOfTheDay(Today obj)
        {
            Dal dal = new Dal();
            List<Item> exist = (from x in dal.items where x.name.Equals(obj.dishoftheday) select x).ToList<Item>();
            if (exist.Count == 0)
            {
                TempData["msg"] = "This item is not exist !!!";
                TempData["color"] = "red";
                return View("AddItemOfDay");
            }
            List<Today> todays = (from x in dal.todays
                                  select x).ToList<Today>();
            todays[0].isRainy = obj.isRainy;
            todays[0].dishoftheday = obj.dishoftheday;
            todays[0].isParty = obj.isParty;
            dal.SaveChanges();
            TempData["msg"] = "the dish of the day added successfully !!!";
            TempData["color"] = "blue";
            return View("AddItemOfDay");
        }
        
           
    }
}