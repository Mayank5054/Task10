using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Models;
using Task10.ViewModels;
using Task10.Filters;

namespace Task10.Controllers
{
    [LoginFilter]
    [AuthorizationFilter(new string[] { "customer" })]
    public class EventItemsController : Controller
    {
        private readonly ExamEntities db;

        public EventItemsController()
        {
            db = new ExamEntities();
        }
        // GET: EventItems
        public ActionResult Index(int id)
        {
            Event ev = db.Events.Where(e => e.EventId == id && e.IsDeleted == 0).FirstOrDefault();
            if (ev == null)
            {
                return HttpNotFound();
            }

            EventItemsProducts vmodel = new EventItemsProducts();

            List<EventItem> ei = db.EventItems.Where(e => e.EventId == id && e.IsDeleted == 0 && (e.Product.Type == "DishItem" || e.Product.Type == "Purchasable")).ToList();
            List<Product> pd = db.Products.Where(p => (p.Type == "DishItem" || p.Type == "Purchasable") && p.Isdeleted == 0).ToList();

            vmodel.eventItems = ei;
            vmodel.products = pd;
            vmodel.EventId = id;

            return View(vmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection collection)
        {
            var checkedProducts = collection.Keys;

            foreach (var item in checkedProducts)
            {
                if (item.ToString().StartsWith("product-") && !item.ToString().EndsWith("Quantity"))
                {
                    int productId = Convert.ToInt32(item.ToString().Split('-')[1]);
                    int eventId = Convert.ToInt32(collection["EventId"]);

                    EventItem eventItem = db.EventItems.Where(e => e.ItemId == productId && e.IsDeleted == 0 && e.EventId == eventId).FirstOrDefault();

                    Product product = db.Products.Find(productId);

                    if (eventItem == null)
                    {

                        if (product.Type.ToLower() != "purchasable" && product.Type.ToLower() != "dishitem")
                        {
                            ViewBag.Error = "Only Purchasable and DishItem are allowed.";
                            return RedirectToAction("Index", "EventItems", new { id = collection["EventId"] });
                        }
                        else
                        {
                            db.EventItems.Add(new EventItem
                            {
                                EventId = eventId,
                                ItemId = productId,
                                Qty = Convert.ToInt32(collection[$"product-{productId}-Quantity"]),
                                TotalAmount = product.Type == "Purchasable" ?
                                (decimal)(Convert.ToDouble(product.Price) - (Convert.ToDouble(product.Price) * 0.05)) * Convert.ToDecimal(collection[$"product-{productId}-Quantity"])
                                : product.Price * Convert.ToDecimal(collection[$"product-{productId}-Quantity"]),
                                CreatedOn = DateTime.Now,
                                UpdatedOn = DateTime.Now,
                                CreatedBy = Convert.ToInt32(Session["UserId"]),
                                IsDeleted = 0,
                            });
                        }
                    }
                    else
                    {
                        eventItem.Qty = Convert.ToInt32(collection[$"product-{productId}-Quantity"]);
                        eventItem.TotalAmount = product.Type == "Purchasable" ?
                                (decimal)(Convert.ToDouble(product.Price) - (Convert.ToDouble(product.Price) * 0.05)) * Convert.ToDecimal(collection[$"product-{productId}-Quantity"])
                                : product.Price * Convert.ToDecimal(collection[$"product-{productId}-Quantity"]);
                        eventItem.UpdatedOn = DateTime.Now;
                    }
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "EventItems", new { id = collection["EventId"] });
        }

        public ActionResult Delete(int id)
        {
            EventItem e = db.EventItems.Find(id);
            if (e == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                //db.Events.Remove(e);
                //db.SaveChanges();

                e.IsDeleted = 1;
                db.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}