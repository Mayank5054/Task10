using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Filters;
using Task10.Models;

namespace Task10.Controllers
{
    [LoginFilter]
    public class CartController : Controller
    {
        ExamEntities _db;
        public CartController()
        {
            _db = new ExamEntities();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadCart(int? start, int? length)
        {

            var userid = Convert.ToInt32(Request["id"]);
            var cart = _db.Carts.FirstOrDefault(x => x.UserId == userid && x.Isdeleted != 1);

            if (cart == null)
            {
                return Json(new
                {
                    status = "Success",
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = (object)null
                }, JsonRequestBehavior.AllowGet);
            }

            List<CartItemId> cartitem = _db.CartItemIds.Where(x => x.CartId == cart.CartId && x.Isdeleted != 1).ToList();

            List<object> items = new List<object>();
            foreach (var item in cartitem)
            {
                items.Add(new
                {
                    id = item.CartItemId1,
                    name = item.Product.Name,
                    qty = item.Qty,
                    total = item.TotalAmount
                });

            }
            int itemscount = items.Count();
            var ite = items;


            if (start != null && length != null)
            {
                if (length > 0)
                {
                    ite = items.Skip((int)start).Take((int)length).ToList();

                }
                else
                {
                    ite = items.Skip((int)start).Take(itemscount).ToList();
                }
            }


            return Json(new
            {
                status = "Success",
                recordsTotal = itemscount,
                recordsFiltered = itemscount,
                data = ite,
            }, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter(new string[] { "customer" })]
        public ActionResult AddtoCart(int id, decimal price)
        {
            int uid = int.Parse(Session["UserId"].ToString());
            Product _product = _db.Products.Where(x=>x.ProductId==id).FirstOrDefault();
            if(_product!=null && _product.Type == "Dish")
            {
                Dish _dish = _db.Dishes.Where(x => (
 x.Date.Value.Year == DateTime.Now.Year &&
  x.Date.Value.Month == DateTime.Now.Month &&
   x.Date.Value.Day == DateTime.Now.Day
 )).FirstOrDefault();

                if (_dish == null || _dish.ItemID != id)
                {
                    return Json(new { status = "failure", message = "dish is not for today" }, JsonRequestBehavior.AllowGet);
                }
                int isAvailable = (int)_dish.DishCount;

                List<Order> todaysOrders =
              _db.Orders.Include("OrderItems").Where(x => (
     x.DateOfOrder.Value.Year == DateTime.Now.Year &&
      x.DateOfOrder.Value.Month == DateTime.Now.Month &&
       x.DateOfOrder.Value.Day == DateTime.Now.Day
     )).ToList();
                int count = 0;
                foreach (Order i in todaysOrders)
                {
                    if (i.OrderItems.Count > 2)
                    {
                        count++;
                    }
                }
                if (isAvailable <= count)
                {
                    return Json(new { status = "failure", message = "Dish Order Limits Reached`" }, JsonRequestBehavior.AllowGet);
                }
            }
       
            var cart = _db.Carts.FirstOrDefault(x => x.UserId == uid && x.Isdeleted != 1);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = uid,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    Isdeleted = 0,
                };
                _db.Carts.Add(cart);
                _db.SaveChanges();

                int newCartId = cart.CartId;


                _db.CartItemIds.Add(new CartItemId
                {
                    CartId = newCartId,
                    ItemID = id,
                    Qty = 1,
                    Amount = price,
                    TotalAmount = price,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    Isdeleted = 0
                });
                _db.SaveChanges();
            }
            else
            {
                int CartId = cart.CartId;

                var ItemId = _db.CartItemIds.FirstOrDefault(x => x.ItemID == id && x.CartId == CartId && x.Isdeleted != 1);
                if (ItemId == null)
                {
                    _db.CartItemIds.Add(new CartItemId
                    {
                        CartId = CartId,
                        ItemID = id,
                        Qty = 1,
                        Amount = price,
                        TotalAmount = price,
                        Isdeleted = 0,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now,

                    });
                }
                else
                {
                    ItemId.Qty++;
                    ItemId.TotalAmount = ItemId.TotalAmount + price;
                }
                _db.SaveChanges();


            }

            return Json(new {status="success",message="Item Added To Cart"},JsonRequestBehavior.AllowGet);
        }
        [AuthorizationFilter(new string[] { "customer" })]
        public ActionResult MinustoCart(int id)
        {
            var cart = _db.CartItemIds.Find(id);
            if (cart.Qty > 1)
            {
                cart.Qty--;
                cart.TotalAmount = cart.TotalAmount - cart.Amount;
            }
            else
            {
                cart.Isdeleted = 1;

            }
            _db.SaveChanges();
            return Json(new { status = "success", message = "Item -1 From Cart" }, JsonRequestBehavior.AllowGet);


        }
        [AuthorizationFilter(new string[] { "customer" })]
        public ActionResult PlustoCart(int id)
        {
            var cart = _db.CartItemIds.Find(id);

            cart.Qty++;
            cart.TotalAmount = cart.TotalAmount + cart.Amount;
            _db.SaveChanges();
            return Json(new { status = "success", message = "Item +1  From Cart" }, JsonRequestBehavior.AllowGet);


        }
        [AuthorizationFilter(new string[] { "customer" })]
        public ActionResult Delete(int id)
        {
            var item = _db.CartItemIds.Find(id);
            item.Isdeleted = 1;
            _db.SaveChanges();
            return Json(new { status = "success", message = "Item Deleted From Cart" }, JsonRequestBehavior.AllowGet);
        }
    }
}