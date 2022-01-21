using CofeeShop.DAL;
using CofeeShop.Models;
using CofeeShop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;



namespace CofeeShop.Controllers
{
    public class UserController : Controller
    {

        // GET: User
        
        public ActionResult HomePage()
        {
            Dal dal = new Dal();
            ItemViewModel cvm = new ItemViewModel();
            List<Item> items = dal.items.ToList<Item>();
            cvm.item = new Item();
            cvm.items = items;
            if (Session["UserName"] == null)
            {
                Guest g = new Guest() { };
                dal.guests.Add(g);
                dal.SaveChanges();
                List<Guest> guests = dal.guests.ToList<Guest>();
                Session["UserName"] = "??" + guests[guests.Count - 1].Id.ToString();
            }
            return View(cvm);
        }
        
        public ActionResult AddNewUser(User obj)
        {
            Dal dal = new Dal();
            List<User> exist = (from x in dal.users where x.userName.Contains(obj.userName) select x).ToList<User>();
            if (exist.Count != 0)
            {
                TempData["msg"] = "User already exist !!";
                TempData["color"] = "red";
                return View("RegisterUsers");
            }

            TempData["msg"] = "User created successfully !!!";
            TempData["color"] = "blue";
            dal.users.Add(obj);
            dal.SaveChanges();
            return View("RegisterUsers");
        }
        public ActionResult LoginUsers()
        {
            return View();
        }
        public ActionResult RegisterUsers()
        {
            return View();
        }
        public ActionResult SignOut()
        {
            Session["UserName"] = null;
            return RedirectToAction("HomePage", "User");
        }
        public ActionResult SingIn(User obj)
        {
            Dal dal = new Dal();
            List<User> exist = (from x in dal.users
                                where x.userName.Equals(obj.userName)
            && x.password.Equals(obj.password)
                                select x).ToList<User>();
            if (exist.Count == 0)
            {
                TempData["msg"] = "Wrong information !!";
                TempData["color"] = "red";
                return View("LoginUsers");
            }

            if (exist[0].userType == "A")
                return RedirectToAction("MangmentItems", "Admin");
            if (exist[0].userType == "B")
                return RedirectToAction("ShowAllOrders", "Baristas");
            else
            {
                Session["UserName"] = obj.userName;
                return RedirectToAction("HomePage", "User");
            }
        }
        public ActionResult Cart()
        {
            Dal dal = new Dal();
            string UserName = (string)Session["UserName"];

            OrderViewModel cvm = new OrderViewModel();
            List<Order> orders = (from x in dal.orders
                                    where x.userName.Equals(UserName)
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
            cvm.orders = last;
            return View(cvm);
        }
        public ActionResult PaymentAll()
        {
            Dal dal = new Dal();
            List<Today> todyss = (from x in dal.todays
                                  select x).ToList<Today>();
            bool n = todyss[0].isParty;
            bool nn = todyss[0].isRainy;
            if (n || nn)
            {
                TempData["msg"] = "we are closed today you cant buy anything!!";
                TempData["color"] = "red";
                return RedirectToAction("HomePage", "User");
            }
            return View();
        }
        public ActionResult BuyAllOrders()
        {

            string id = (string)Session["UserName"];
            Dal dal = new Dal();
            List<Order> orders = (from x in dal.orders
                                  where x.userName.Equals(id)
                                  select x).ToList<Order>();

            foreach (Order order in orders)
                order.isPayed = true;

            dal.SaveChanges();
            if (orders[0].tableId < 10)
            {
                TempData["msg"] = "Payment done successfully !!";
                TempData["color"] = "blue";
                return RedirectToAction("Cart", "User");
            }
            TempData["msg"] = "Payment done successfully !!";
            TempData["color"] = "blue";
            return RedirectToAction("Cart", "User");

        }
        public ActionResult Payment()
        {
            Dal dal = new Dal();
            Session["BuyOrder"] = Request.Form["ordertId1"];
            List<Today> todyss = (from x in dal.todays
                                  select x).ToList<Today>();
            bool n = todyss[0].isParty;
            bool nn = todyss[0].isRainy;
            if (n || nn)
            {
                TempData["msg"] = "we are closed today you cant buy anything!!";
                TempData["color"] = "red";
                return RedirectToAction("HomePage", "User");
            }
            return View();
        }
        public ActionResult BuyOrder()
        {
            string UserName = (string)Session["UserName"];
            int id;
            Int32.TryParse((string)Session["BuyOrder"], out id);
            Dal dal = new Dal();
            List<Order> orders = (from x in dal.orders
                                    where x.orderId.Equals(id)
                                    select x).ToList<Order>();
            orders[0].isPayed = true;
            dal.SaveChanges();
                TempData["msg"] = "Payment done successfully !!";
                TempData["color"] = "blue";
                return RedirectToAction("Cart", "User");
            
        }
        public ActionResult BookTable()
        {

            TableViewModel cvm = new TableViewModel();
            string UserName = (string)Session["UserName"];
            Dal dal = new Dal();
            List<Table> tables = (from x in dal.tables from y in dal.orders
                                  where x.tableId!=y.tableId
                                 select x).ToList<Table>();
            Session["max"] = tables[0].seatsNumber;
            var not = (from x in dal.tables
                       from y in dal.orders
                       where x.tableId == y.tableId 
                       select x).ToList<Table>();
            var all = (from x in dal.tables
                       select x).ToList<Table>();
            List<Order> orders = (from x in dal.orders
                                  where x.userName == UserName
                                  select x).ToList<Order>();
            List<Table> ok = all.Except(not).ToList();
            List<Table> last = ok.Distinct().ToList();
            foreach (Order o in orders) {
                if (orders != null && o.isPayed == false)
                {

                    cvm.table = new Table();
                    cvm.tables = last;
                    return View(cvm);
                }
            }
            TempData["msg"] = "there are no orders you have asked for!!";
            TempData["color"] = "red";
            return RedirectToAction("HomePage","User");
        }
        public ActionResult CreateTable()
        {
            string UserName = (string)Session["UserName"];
            int id, tablenum;
            Int32.TryParse(Request.Form["TableNum"], out tablenum);
            Dal dal = new Dal();
            List<User> exist = (from x in dal.users where x.userName.Equals(UserName) select x).ToList<User>();
            List<Order> last = (from x in dal.orders where x.userName.Equals(UserName) select x).ToList<Order>();
            last[last.Count].tableId = tablenum;
            dal.SaveChanges();
            TempData["msg"] = "the table have been booked!!";
            TempData["color"] = "blue";
            return RedirectToAction("HomePage", "User");
        }
        public ActionResult FilterItem()
        {

            Dal dal = new Dal();
            ItemViewModel cvm = new ItemViewModel();
            List<Item> items = (from x in dal.items
                                  where x.prePrice != 0
                                  select x).ToList<Item>();
            cvm.item = new Item();
            cvm.items = items;
            return View("HomePage", cvm);
        }
        public ActionResult OrderItems()
        {
            Dal dal = new Dal();
            ItemViewModel cvm = new ItemViewModel();
            string choice = (string)Request.Form["OrderBy"];
            List<Item> items;
            switch (choice)
            {
                case "Price increase":
                    items = dal.items.OrderBy(c => c.price).ToList<Item>();
                    break;
                case "Price decrease":
                    items = dal.items.OrderByDescending(x => x.price).ToList<Item>();
                    break;
                case "Most popular":
                    int i, j, min;
                    Item temp;
                    List<Item> Allitems = dal.items.ToList<Item>();
                    for (i = 0; i < Allitems.Count - 1; i++)
                    {
                        min = i;
                        for (j = i + 1; j < Allitems.Count; j++)
                            if (GetPopular(Allitems[j].itemId) >= GetPopular(Allitems[min].itemId))
                                min = j;
                        temp = Allitems[i];
                        Allitems[i] = Allitems[min];
                        Allitems[min] = temp;
                    }
                    items = Allitems;
                    break;
                case "item of the day":
                    List<Today> tod = (from x in dal.todays
                                          select x).ToList<Today>();
                    string str = tod[0].dishoftheday;
                    List<Item> itemss = (from x in dal.items where x.name == str select x).ToList<Item>();
                    items = itemss;
                    break;
                default:
                    items = dal.items.OrderBy(x => x.category).ToList<Item>();
                    break;
            }
            cvm.item = new Item();
            cvm.items = items;
            return View("HomePage", cvm);
        }
        public int GetPopular(int id)
        {
            Dal dal = new Dal();
            List<Order> orders = (from x in dal.orders where x.orderId.Equals(id) select x).ToList<Order>();
            return orders.Count;
        }
        public ActionResult SearchItem()
        {
            return View();
        }
        public ActionResult SearchByCategory()
        {

            Dal dal = new Dal();
            string category = Request.Form["ItemCategory"];
            ItemViewModel cvm = new ItemViewModel();
            List<Item> movies = (from x in dal.items
                                  where x.category.Equals(category)
                                  select x).ToList<Item>();
            cvm.item = new Item();
            cvm.items = movies;
            return View("HomePage", cvm);
        }
        public ActionResult SearchByName()
        {

            Dal dal = new Dal();
            string nam = Request.Form["ItemName"];
            ItemViewModel cvm = new ItemViewModel();
            List<Item> items = (from x in dal.items
                                  where x.name.Equals(nam)
                                  select x).ToList<Item>();
            cvm.item = new Item();
            cvm.items = items;
            return View("HomePage", cvm);
        }
        public ActionResult SearchByPrice()
        {

            Dal dal = new Dal();
            double price;
            double.TryParse(Request.Form["rangeInput"], out price);
            ItemViewModel cvm = new ItemViewModel();
            List<Item> items = (from x in dal.items
                                  where x.price <= price
                                  select x).ToList<Item>();
            cvm.item = new Item();
            cvm.items = items;
            return View("HomePage", cvm);
        }
        public ActionResult Once()
        {

            var dal = new Dal();

            var itemmId = int.Parse(Request.Form["itemId"]);
            List<Item> items = (from x in dal.items
                                where x.itemId == itemmId
                                select x).ToList<Item>();
            Session["Itemid"] = items[0].itemId;
            ItemViewModel cvm = new ItemViewModel();
            cvm.item = new Item();
            cvm.items = items;
            List<Today> todyss = (from x in dal.todays
                                select x).ToList<Today>();
            bool n = todyss[0].isParty;
            bool nn= todyss[0].isRainy;
            if(n||nn)
            {
                TempData["msg"] = "we are closed today you cant buy anything!!";
                TempData["color"] = "red";
                return RedirectToAction("HomePage", "User");
            }
            if (items[0].isAvailable ==false)

            {
                TempData["msg"] = "you Can't Buy this item , It is unavailable yet.!!";
                TempData["color"] = "red";
                return RedirectToAction("HomePage", "User"); 
            }
            if (Session["age"] != null && items[0].age > (int)Session["age"])

            {
                TempData["msg"] = "You are under the requested age !!";
                TempData["color"] = "red";

                return RedirectToAction("HomePage", "User"); 
            }
            return View(cvm);
        }
        public ActionResult CreateOrde()
        {
            int id;
            id = (int)Session["Itemid"];
            Dal dal = new Dal();
            Order order = new Order
            { isPayed = false,  itemId = id, userName = (string)Session["UserName"] };

            dal.orders.Add(order);
            dal.SaveChanges();
            TempData["msg"] = "Item added to Cart Susseccfuly!!";
            TempData["color"] = "blue";
            return RedirectToAction("HomePage", "User");
        }
        
            public ActionResult CheckTable(Table obj)
        {
            TableViewModel cvm = new TableViewModel();
            

            Dal dal = new Dal();
            var zz = (from x in dal.tables
                      from y in dal.orders
                      where x.tableId == y.tableId
                      select x).ToList<Table>();
            var all = (from x in dal.tables
                       select x).ToList<Table>();
            List<Table> ok = all.Except(zz).ToList();
            var not = (from x in ok
                                  from y in dal.orders
                                  where x.tableId != y.tableId
                       select x).ToList<Table>();
            not = (from x in not
                        where x.status == obj.status 
                        select x).ToList<Table>();
            not = (from x in not
                     where x.seatsNumber >= obj.seatsNumber
                    select x).ToList<Table>();
            
            List<Table> last = not.Distinct().ToList();

            cvm.table = new Table();
            cvm.tables = last;
            return View("BookTable", cvm);
        }
        
            public ActionResult Register(Table obj)
        {
            string UserName = (string)Session["UserName"];

            Dal dal = new Dal();
            List<Order> orders = (from x in dal.orders 
                                  where x.userName== UserName 
                                  select x).ToList<Order>();
            List<Order> z = (from x in dal.orders
                                  select x).ToList<Order>();
            
                foreach (Order x in z)
                    if (obj.tableId == x.tableId)
                    {
                        TempData["msg"] = "Theis table is already taken !!!";
                        TempData["color"] = "red";
                        return RedirectToAction("HomePage", "User");
                    }
                        
            //add an error message!
            orders[orders.Count - 1].tableId = obj.tableId;
            dal.SaveChanges();
            //orders[].tableId = obj.tableId;
            
            TempData["msg"] = "The table have been booked successfully !!!";
            TempData["color"] = "green"; 
            return RedirectToAction("HomePage", "User");
        }
    }
}
