using Task10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Migrations;
using Task10.Filters;
using Task10.Models;
namespace MVC_EXAM_TASK.Controllers
{

    public class ProductController : Controller
    {
        ExamEntities _db = new ExamEntities();
        // GET: Product

        public ActionResult AllProducts()
        {
            try
            {
                List<Product> products = _db.Products.Where(p => (p.Type == "Purchasable" || p.Type == "Dish") && p.Isdeleted == 0 && p.Visible == true).ToList();
                return View("UserAllProducts", products);
            }
            catch (Exception error)
            {
                return RedirectToAction("Error", "User", new { error = error.Message });
            }
        }

        //Product Add
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product.CreatedOn = DateTime.Now;
                    product.UpdatedOn = DateTime.Now;
                    product.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    product.Isdeleted = 0;

                    _db.Products.Add(product);
                    _db.SaveChanges();
                    return RedirectToAction("Dashbord", "Admin");
                }
                else
                {

                    return View("AddProduct", product);
                }
            }
            catch (Exception error)
            {
                return RedirectToAction("Error", "User", new { error = error.Message });
            }

        }


        //Product Update
        public ActionResult UpdateProduct(int id)
        {
            try
            {
                Product product = _db.Products.Find(id);
                return View(product);
            }
            catch (Exception error)
            {
                return RedirectToAction("Error", "User", new { error = error.Message });
            }

        }

        [HttpPost]
        public ActionResult UpdateProduct(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product.UpdatedOn = DateTime.Now;
                    _db.Products.AddOrUpdate(product);
                    _db.SaveChanges();
                    return RedirectToAction("Dashbord", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Error");
                    return View("UpdateProduct", product);
                }
            }
            catch (Exception error)
            {
                return RedirectToAction("Error", "User", new { error = error.Message });
            }

        }

        //Delete Product(Soft delete)
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                Product product = _db.Products.Find(id);
                product.Isdeleted = 1;
                _db.Products.AddOrUpdate(product);
                _db.SaveChanges();
                return RedirectToAction("Dashbord", "Admin");
            }
            catch (Exception error)
            {
                return RedirectToAction("Error", "User", new { error = error.Message });
            }

        }
    }
}