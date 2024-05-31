using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Filters;
using Task10.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data.Entity.Migrations;
namespace Task10.Controllers
{
    [LoginFilter]
    public class HomeController : Controller
    {
        ExamEntities _db;
        public HomeController()
        {
            _db = new ExamEntities();
        }

        //[AuthorizationFilter(new string[] { "cashier" })]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
       
        public ActionResult AllProducts() {
            List<Product> _products = _db.Products.Where(x => (x.Isdeleted !=1 && (x.Type == "Purchasable" || (x.Type == "Dish" ))) ).ToList();
            return View(_products);
        }
      
        public ActionResult TodaysDish()
        {
           string _ans=  DateTime.UtcNow.ToShortDateString().ToString();
            //DateTime _V2 = x.Date;
           Dish _dishes =_db.Dishes.Where(x=> (
           x.Isdeleted != 1 &&
            x.Date.Value.Year == DateTime.Now.Year &&
             x.Date.Value.Month == DateTime.Now.Month &&
              x.Date.Value.Day == DateTime.Now.Day 
            )).FirstOrDefault();
           
            return View(_dishes);
        }

        [AuthorizationFilter(new string[] { "cashier" })]
        public ActionResult  MakeAnOrder(int productId)
        {
            Product _product = _db.Products.Where(x=>x.ProductId == productId).FirstOrDefault();
            if(_product != null )
            {
                int userId = int.Parse(Session["UserId"].ToString());
                if(_product.Type == "Dish")
                {
                    //Dish _dish = _db.Dishes.Where(x=>x.ItemID == _product.ProductId).FirstOrDefault();

                    //int isAvailable = (int)_dish.DishCount;

                    //List<int> _itemIds = _db.DishItems.Where(x => x.DishId == _dish.DishId)
                    //    .Select(x => (int)x.ItemID).ToList();

                    //int countOrders = _db.OrderItems.Where(x => _itemIds.Contains((int)x.ItemID)).Count();

                    //countOrders = (int)countOrders / _itemIds.Count();
                   
                    Dish _dish = _db.Dishes.Where(x => (
         x.Date.Value.Year == DateTime.Now.Year &&
          x.Date.Value.Month == DateTime.Now.Month &&
           x.Date.Value.Day == DateTime.Now.Day
         )).FirstOrDefault();

                    if(_dish == null || _dish.ItemID != _product.ProductId)
                    {
                        return Json(new { status="failure", message = "dish is not for today" }, JsonRequestBehavior.AllowGet);
                    }
                    int isAvailable = (int)_dish.DishCount;

                    List<Order> todaysOrders =
                  _db.Orders.Include("OrderItems").Where(x => (
         x.DateOfOrder.Value.Year == DateTime.Now.Year &&
          x.DateOfOrder.Value.Month == DateTime.Now.Month &&
           x.DateOfOrder.Value.Day == DateTime.Now.Day
         )).ToList();
                    int count = 0;
                    foreach(Order i in todaysOrders)
                    {
                        if(i.OrderItems.Count > 2)
                        {
                            count++;
                        }
                    }


                    
                    if (isAvailable <= count)
                    {
                        return Json(new { status = "failure", message = "Dish Order Limits Reached`" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<DishItem> _dishList = _db.DishItems.Where(x => x.DishId == _dish.DishId).ToList();
                        int totalAmount = (int)_dishList.Sum(x => x.Product.Price);
                        Order _order = new Order();
                        _order.CustomerId = userId;
                        _order.TotalAmount = _product.Price;
                        _order.DateOfOrder = DateTime.Now;
                        _order.Type = "Single";
                        _order.CreatedOn = DateTime.Now;
                        _order.UpdatedOn = DateTime.Now;
                        _db.Orders.Add(_order);
                        _db.SaveChanges();
                        foreach (DishItem i in _dishList)
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem.OrderId = _order.OrderId;
                            orderItem.ItemID = i.Product.ProductId;
                            orderItem.Type = i.Product.Type;
                            orderItem.Quantity = i.Qty;
                            orderItem.Amount = i.Product.Price;
                            orderItem.TaxPercentage = 0;
                            orderItem.TaxAmount = 0;
                            orderItem.Discount = 0;
                            orderItem.DiscountId = null;
                            orderItem.CreatedOn = DateTime.Now;
                            orderItem.UpdatedOn = DateTime.Now;
                            orderItem.CreatedBy = null;
                            orderItem.Isdeleted = 0;

                            _db.OrderItems.Add(orderItem);
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    
                        Order _order = new Order();
                        _order.CustomerId = userId;
                        _order.TotalAmount = _product.Price;
                        _order.DateOfOrder = DateTime.Now;
                        _order.Type = "Single";
                        _order.CreatedOn = DateTime.Now;
                        _order.UpdatedOn = DateTime.Now;
                        _db.Orders.Add(_order);
                        _db.SaveChanges();

                        OrderItem orderItem = new OrderItem();
                        orderItem.OrderId = _order.OrderId;
                        orderItem.ItemID = _product.ProductId;
                        orderItem.Type = _product.Type;
                        orderItem.Quantity = 1;
                        orderItem.Amount = _product.Price;
                        orderItem.TaxPercentage = 0;
                        orderItem.TaxAmount = 0;
                        orderItem.Discount = 0;
                        orderItem.DiscountId = null;
                        orderItem.CreatedOn = DateTime.Now;
                        orderItem.UpdatedOn = DateTime.Now;
                        orderItem.CreatedBy = null;
                        orderItem.Isdeleted = 0;

                        _db.OrderItems.Add(orderItem);
                        _db.SaveChanges();
                    
                }
                return Json(new { status = "success", message = "Order Has been placed" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "failure", message = "Product or Dish Is Not Available" }, JsonRequestBehavior.AllowGet);
           
            
        }
       
        public ActionResult GetAllOrders()
        {
            int userId = int.Parse(Session["UserId"].ToString());
            List<Order> _orders = _db.Orders.Where(x=>x.CustomerId == userId).Include("OrderItems").ToList();


            List<DisplayOrder> _listOfDisplay = new List<DisplayOrder>();
            foreach(Order i in _orders)
            {
                DisplayOrder _do = new DisplayOrder();
                _do.OrderId = i.OrderId;
                _do.orderDate = (DateTime)i.DateOfOrder;
                _do.Amount = (int)i.TotalAmount;

                if (i.OrderItems.Count > 1)
                {

                    //int id = (int)i.OrderItems.FirstOrDefault().ItemID;
                    //int DishId = (int)_db.DishItems.Where(x=>x.ItemID == id).FirstOrDefault().DishId;
                    //Dish _dish = _db.Dishes.Where(x=>x.DishId == DishId).FirstOrDefault();
      
                    //Dish _dish = _db.Dishes.Where(x => DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(i.DateOfOrder)).FirstOrDefault();

                    Dish _dish = _db.Dishes.Where(x => (
        x.Date.Value.Year == i.DateOfOrder.Value.Year &&
         x.Date.Value.Month == i.DateOfOrder.Value.Month &&
          x.Date.Value.Day == i.DateOfOrder.Value.Day
        )).FirstOrDefault();
                    if(_dish == null)
                    {
                        _do.singleProduct = null;
                    }
                    else
                    {
                        _do.singleProduct = _dish.Product.Name;
                    }
              
                }
                else if(i.OrderItems.Count == 0)
                {
                    _do.singleProduct = "Not Found";
                }
                else
                {
                    _do.singleProduct = i.OrderItems.FirstOrDefault().Product.Name;
                }
                _listOfDisplay.Add(_do);
            }
            return View(_listOfDisplay);
        }

        public ActionResult GetOrderById(int orderId)
        {
            Order _order = _db.Orders.Where(x=>x.OrderId==orderId).Include("OrderItems").FirstOrDefault();
            DisplayOrder _do = new DisplayOrder();
            _do.OrderId = _order.OrderId;
            _do.orderDate = (DateTime)_order.DateOfOrder;

            List<string> products = new List<string>();
            foreach(OrderItem i in _order.OrderItems)
            {
                products.Add(i.Product.Name);
            }
            _do.ProductName = products;
            string data = JsonConvert.SerializeObject(_do);
            return Json(new { status = "success",data =data}, JsonRequestBehavior.AllowGet);
        }





        ////////////////////////////////////////////////   Chintan Task (Task 12) //////////////////////
        public ActionResult Index()
        {
            var data = _db.DineInTables.Where(x => x.CanBeReserved == true).ToList();

            return View(data);
        }

        public ActionResult CreateTablebyAdmin()
        {
            return View();
        }

        public ActionResult EditTable(int? id)
        {
            var data = _db.DineInTables.Where(x => x.TableId == id).FirstOrDefault();
            return View(data);
        }


        public ActionResult DeleteTable(int? id)
        {
            var res_data = _db.ReservedTables.Where(x => x.TableId == id).ToList();

            foreach (var i in res_data)
            {
                var d = _db.ReservedTables.Where(x => x.TableId == @i.TableId).FirstOrDefault();
                _db.ReservedTables.Remove(d);
                _db.SaveChanges();
            }


            var data = _db.DineInTables.Find(id);
            _db.DineInTables.Remove(data);
            _db.SaveChanges();
            return RedirectToAction("AllTableAdmin", "Home");
        }

        public ActionResult UpdateTable(DineInTable data, string CanBeReserved)
        {
            if (CanBeReserved == "Yes")
            {
                data.CanBeReserved = true;
            }
            else
            {
                data.CanBeReserved = false;
            }

            _db.DineInTables.AddOrUpdate(data);
            _db.SaveChanges();
            return RedirectToAction("AllTableAdmin", "Home");
        }

        [HttpPost]
        public ActionResult SaveTablebyAdmin(DineInTable data, string CanBeReserved)
        {
            if (CanBeReserved == "Yes")
            {
                data.CanBeReserved = true;
            }
            else
            {
                data.CanBeReserved = false;
            }

            _db.DineInTables.Add(data);
            _db.SaveChanges();

            return RedirectToAction("AllTableAdmin", "Home");
        }
        //public ActionResult CreateTableByAdmin(DineInTable dta)
        //{
        //    return View();
        //} 

        public ActionResult AllTableAdmin()
        {
            List<DineInTable> data = _db.DineInTables.ToList();
            return View(data);
        }
    }
}