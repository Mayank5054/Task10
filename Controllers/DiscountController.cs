using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Task10.Models;

namespace Task10.Controllers
{
    public class DiscountController : Controller
    {
        private ExamEntities _dbContext;

        public DiscountController()
        {
            _dbContext = new ExamEntities();
        }

        public ActionResult discounts()
        {
            int adminId = (int)Session["UserId"];
            var discount = _dbContext.Discounts.Where(x => x.CreatedBy == adminId && x.IsDeleted == 0).ToList();
            return View("discounts", discount);
        }

        public ActionResult AddOrEditDiscount(int? id)
        {
            if (id == 0)
            {
                Discount discount = new Discount();
                return View("AddOrEditDiscount", discount);
            }
            else
            {
                var discount = _dbContext.Discounts.FirstOrDefault(x => x.DiscountId == id);
                return View("AddOrEditDiscount", discount);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveDiscount(Discount model)
        {
            ModelState.Remove("CouponName");
            if (ModelState.IsValid)
            {
                bool bIsCouponExist = _dbContext.Discounts.Any(m => m.CouponName == model.CouponName);
                if (bIsCouponExist && model.DiscountId == 0)
                {
                    ModelState.AddModelError("CouponName", "Please enter a unique coupon name");
                }
                if (model.To < model.From)
                {
                    ModelState.AddModelError("From", "The starting date should not be greater than the ending date");
                }

                if (ModelState.IsValid)
                {
                    if (model.DiscountId == 0)
                    {
                        model.CreatedOn = DateTime.Now;
                        model.IsDeleted = 0;
                        model.CreatedBy = (int)Session["UserId"];
                    }
                    else
                    {
                        var disObj = _dbContext.Discounts.FirstOrDefault(x => x.DiscountId == model.DiscountId);
                        model.CreatedOn = disObj.CreatedOn;
                        model.CreatedBy = disObj.CreatedBy;
                        model.CouponName = disObj.CouponName;
                        model.IsDeleted = disObj.IsDeleted;
                        model.UpdatedOn = DateTime.Now;
                    }
                    _dbContext.Discounts.AddOrUpdate(model);
                    _dbContext.SaveChanges();
                    TempData["DiscountMsg"] = "Save Discount Successfully!!";
                    return Json(new { success = true });
                }
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList();
            return Json(new { success = false, errors = errorList });
        }


        public ActionResult DeleteDiscount(int? id)
        {
            if (id != null)
            {
                var discountObj = _dbContext.Discounts.Find(id);
                if (discountObj != null)
                {
                    _dbContext.Discounts.Remove(discountObj);
                    _dbContext.SaveChanges();
                    TempData["DiscountMsg"] = "Delete Discount Successfully!!";
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, error = "Object not found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "ID is null");
            }
        }
    }
}