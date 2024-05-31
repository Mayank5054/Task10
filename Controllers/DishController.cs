using Task10.Filters;
using Task10.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Task10.Models;

namespace MVC_EXAM_TASK.Controllers
{

    public class DishController : Controller
    {
        ExamEntities _db = new ExamEntities();

        //GET:Dish
        [HttpGet]
        public ActionResult Dishes()
        {
            List<Dish> dishes = _db.Dishes.Where(d => d.Isdeleted == 0).ToList();
            return PartialView("AllDishes", dishes);
        }

        //Fetch dish names from product table
        public ActionResult DishName(string type)
        {
            List<object> itemname = new List<object>();
            if (type == "Dish")
            {
                itemname = _db.Products.Where(p => p.Type == "Dish").Select(p => new
                {
                    Name = p.Name,
                    Id = p.ProductId
                }).ToList<object>();
            }
            else
            {
                itemname = _db.Products.Where(e => e.Type == "Dishitem" || e.Type == "Purchasable").Select(p => new
                {
                    Name = p.Name,
                    Id = p.ProductId
                }).ToList<object>();
            }

            return Json(itemname, JsonRequestBehavior.AllowGet);
        }

        //fetch products from product table
        public ActionResult GetProduct(string type)
        {
            List<object> products = new List<object>();
            if (type == "Dish")
            {
                products = _db.Products.Where(e => e.Type == "Dishitem" && e.Isdeleted == 0).Select(e => new
                {
                    Id = e.ProductId,
                    Name = e.Name
                }).ToList<object>();
            }
            else
            {
                products = _db.Products.Where(e => e.Type == "Dishitem" || e.Type == "Purchasable" && e.Isdeleted == 0).Select(e => new
                {
                    Id = e.ProductId,
                    Name = e.Name
                }).ToList<object>();
            }
            return Json(products, JsonRequestBehavior.AllowGet);
        }


        //Add New Dish 
        public ActionResult AddDish()
        {
            return View();
        }


        [HttpPost]
        public ActionResult AddDish(Dish dish, string SelectedItems)
        {
            if (ModelState.IsValid)
            {
                //Check that is there any dish avaliable on that date
                Dish dishexist = _db.Dishes.Where(d => d.Date == dish.Date && d.Isdeleted == 0).FirstOrDefault();

                if (dishexist != null)
                {
                    return Json(new { message = "On This Date Dish is alredy avaliable", success = false });
                }
                else
                {
                    //convert Dishitems ARry of object into list of object
                    List<DishItemClass> dishitems = JsonConvert.DeserializeObject<List<DishItemClass>>(SelectedItems);

                    dish.CreatedOn = DateTime.Now;
                    dish.UpdatedOn = DateTime.Now;
                    dish.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    dish.Isdeleted = 0;
                    _db.Dishes.Add(dish);
                    _db.SaveChanges();

                    //fetch new ADded Dish ID
                    int DishId = _db.Dishes.OrderByDescending(d => d.DishId).Select(d => d.DishId).FirstOrDefault();

                    //Store Dishitems into DIshtbale one by one
                    foreach (var item in dishitems)
                    {
                        DishItem dishitem = new DishItem();
                        dishitem.DishId = DishId;
                        dishitem.ItemID = Convert.ToInt32(item.itemId);
                        dishitem.Qty = Convert.ToInt32(item.qty);
                        dishitem.CreatedOn = DateTime.Now;
                        dishitem.UpdatedOn = DateTime.Now;
                        dishitem.CreatedBy = Convert.ToInt32(Session["UserId"]);
                        dishitem.Isdeleted = 0;
                        _db.DishItems.Add(dishitem);
                        _db.SaveChanges();
                    }
                    return Json(new { message = "Dish Added Successfully", success = true });
                }
            }
            else
            {
                return Json(new { message = "All Details are Neccessary", success = false });
            }
        }


        //Update Dish
        public ActionResult UpdateDish(int id)
        {
            Dish dish = _db.Dishes.Find(id);
            return View(dish);
        }


        [HttpPost]
        public ActionResult UpdateDish(Dish dish, string SelectedItems)
        {
            if (ModelState.IsValid)
            {
                //Check that is there any dish avaliable on that date
                Dish dishexist = _db.Dishes.Where(d => d.Date == dish.Date && d.DishId != dish.DishId && dish.Isdeleted == 0).FirstOrDefault();
                if (dishexist != null)
                {
                    return Json(new { message = "On This Date Dish is alredy avaliable", success = false });
                }
                else
                {
                    //convert Dishitems ARry of object into list of object
                    List<DishItemClass> dishitems = JsonConvert.DeserializeObject<List<DishItemClass>>(SelectedItems);
                    dish.UpdatedOn = DateTime.Now;
                    _db.Dishes.AddOrUpdate(dish);
                    _db.SaveChanges();

                    //Store Dishitems into DIshtbale one by one
                    foreach (var item in dishitems)
                    {
                        int itemid = Convert.ToInt32(item.itemId);

                        //If item is already exisits in datatable with same dish
                        DishItem dishitem = new DishItem();
                        dishitem = _db.DishItems.Where(d => d.DishId == dish.DishId && d.ItemID == itemid).FirstOrDefault();

                        //If dish item with dish is  not exist
                        if (dishitem == null)
                        {
                            dishitem = new DishItem
                            {
                                CreatedOn = DateTime.Now,
                                UpdatedOn = DateTime.Now,
                                CreatedBy = dish.CreatedBy,
                                DishId = dish.DishId,
                                ItemID = Convert.ToInt32(item.itemId),
                                Qty = Convert.ToInt32(item.qty),
                                Isdeleted = 0
                            };
                            _db.DishItems.Add(dishitem);
                        }
                        //if dish item with dish is already exist
                        else
                        {
                            //If dish quantity is 0 then set it as deleted
                            if (Convert.ToInt32(item.qty) == 0)
                            {
                                dishitem.Isdeleted = 1;
                            }
                            else
                            {
                                dishitem.Isdeleted = 0;
                            }
                            dishitem.UpdatedOn = DateTime.Now;
                            dishitem.Qty = Convert.ToInt32(item.qty);
                        }
                        _db.SaveChanges();
                    }

                    return Json(new { message = "Dish Updated Successfully", success = true });
                }
            }
            else
            {
                return Json(new { message = "All Field are Not matchwith it's condition", success = false });

            }
        }

        //Delete Dish 
        public ActionResult DeleteDish(int id)
        {
            try
            {
                Dish dish = _db.Dishes.Find(id);
                dish.Isdeleted = 1;
                _db.Dishes.AddOrUpdate(dish);
                _db.SaveChanges();
                return RedirectToAction("Dashbord", "Admin");
            }
            catch (Exception error)
            {
                return RedirectToAction("Error", "User", new { error = error.Message });
            }

        }

        //fetch dishitems from dish table
        public ActionResult Dishitem(int id)
        {
            List<object> items = _db.DishItems.Where(d => d.DishId == id && d.Isdeleted == 0).Select(d => new
            {
                Name = d.Product.Name,
                Qty = d.Qty
            }).ToList<object>();

            return Json(new { data = items }, JsonRequestBehavior.AllowGet);
        }

        

        
    }
}
