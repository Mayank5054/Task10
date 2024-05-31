using Task10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.ViewModels;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Net;
using System.Net.Mail;


namespace ExamTask.Controllers
{
    public class StockController : Controller
    {
        ExamEntities db = new ExamEntities();


        // Constructor without dependency injection
        public StockController()
        {
        }

        //Action to display stock report
        public ActionResult StockReport()
        {

            var user = Session["UserID"];
            if (user != null)
            {
                var stockReport = db.Database.SqlQuery<StockReportViewModel>("EXEC GetStockReport").ToList();
                return View("StockReport", stockReport);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public ActionResult Stocks()
        {
            var stocks = db.Stocks.Where(x => x.IsDeleted == 0).ToList();
            return PartialView("_StockPartial", stocks);
        }

        //public ActionResult AddOrEditStock(int? id)
        //{
        //    List<SelectListItem> itemNames = new List<SelectListItem>();
        //    List<Product> itemList = db.Products.ToList();

        //    itemList.ForEach(x =>
        //    {
        //        itemNames.Add(new SelectListItem { Text = x.Name, Value = x.ProductId.ToString() });
        //    });

        //    if (id == 0)
        //    {
        //        Stock stock = new Stock();
        //        var viewModel = new stockViewModel
        //        {
        //            stock = stock,
        //            ItemNames = itemNames
        //        };
        //        return PartialView("_addOrEditStockPartial", viewModel);
        //    }
        //    else
        //    {
        //        var stock = db.Stocks.FirstOrDefault(x => x.StockId == id);
        //        var viewModel = new stockViewModel
        //        {
        //            stock = stock,
        //            ItemNames = itemNames
        //        };
        //        return PartialView("_addOrEditStockPartial", viewModel);
        //    }
        //}

        public ActionResult AddOrEditStock(int? id)
        {
            List<SelectListItem> itemNames = new List<SelectListItem>();
            List<Product> itemList = db.Products.ToList();

            Stock stock;
            if (id == 0)
            {
                stock = new Stock();
            }
            else
            {
                stock = db.Stocks.FirstOrDefault(x => x.StockId == id);
            }

            itemList.ForEach(x =>
            {
                itemNames.Add(new SelectListItem
                {
                    Text = x.Name,
                    Value = x.ProductId.ToString(),
                    Selected = (stock != null && stock.ItemId == x.ProductId)
                });
            });

            var viewModel = new stockViewModel
            {
                stock = stock,
                ItemNames = itemNames
            };
            return PartialView("_addOrEditStockPartial", viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveStock(stockViewModel model, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var itemId = (int?)Convert.ToUInt32(form["ItemId"]);
                var existingStock = db.Stocks.FirstOrDefault(
                        x => x.ItemId == itemId &&
                             x.Date == model.stock.Date &&
                             x.Type == model.stock.Type
                    );

                if (existingStock != null)
                {
                    existingStock.Qty += model.stock.Qty;
                    existingStock.UpdatedOn = DateTime.Now;

                    if (model.stock.StockId != 0)
                    {
                        var stockToDelete = db.Stocks.FirstOrDefault(x => x.StockId == model.stock.StockId);
                        if (stockToDelete != null)
                        {
                            db.Stocks.Remove(stockToDelete);
                        }
                    }

                    db.Stocks.AddOrUpdate(existingStock);
      
                }

                else
                {
                    if (model.stock.StockId == 0)
                    {
                        var adminId = (int)Session["UserId"];
                        model.stock.CreatedOn = DateTime.Now;
                        model.stock.IsDeleted = 0;
                        model.stock.CreatedBy = adminId;
                        model.stock.ItemId = (int?)Convert.ToUInt32(form["ItemId"]);
                    }
                    else
                    {
                        var stockObj = db.Stocks.FirstOrDefault(x => x.StockId == model.stock.StockId);
                        model.stock.CreatedOn = stockObj.CreatedOn;
                        model.stock.CreatedBy = stockObj.CreatedBy;
                        model.stock.UpdatedOn = DateTime.Now;
                        model.stock.IsDeleted = stockObj.IsDeleted;
                        model.stock.ItemId = (int?)Convert.ToUInt32(form["ItemId"]);
                    }
                    db.Stocks.AddOrUpdate(model.stock);
                }
                db.SaveChanges();
                TempData["StockMsg"] = "Save Data Successfully!!";
                return Json(model.stock);
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong please try again later");
                return Json(null);
            }
        }

        public ActionResult DeleteStock(int? id)
        {
            if (id != null)
            {
                var stockObj = db.Stocks.Find(id);
                if (stockObj != null)
                {
                    stockObj.IsDeleted = 1;
                    //db.Stocks.Remove(stockObj);
                    db.SaveChanges();
                    TempData["StockMsg"] = "Delete Stock Successfully!!";
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

        private List<Product> GetStockReportModel()
        {
            var products = db.Products.Where(x => x.Isdeleted == 0).ToList();
            var orderItems = db.OrderItems.Where(x => x.Isdeleted == 0).ToList();
            var stocks = db.Stocks.Where(x => x.IsDeleted == 0).ToList();

            var stockReportModel = products.Select(product =>
            {
                var totalStock = stocks.Where(x => x.ItemId == product.ProductId && x.Type == "In").Sum(s => s.Qty);
                var outStock = stocks.Where(x => x.ItemId == product.ProductId && x.Type == "Out").Sum(s => s.Qty);
                var orderItemQty = orderItems.Where(oi => oi.ItemID == product.ProductId).Sum(oi => oi.Quantity);
                var remainingStock = totalStock - outStock - orderItemQty;
                return new Product
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    TotalStock = (int)totalStock,
                    RemainingStock = (int)remainingStock,
                    IsLowStock = remainingStock < product.Threshold ? "Yes" : "No"
                };
            }).ToList();

            return stockReportModel;
        }

        //public ActionResult StockReport()
        //{
        //    var stockReportModel = GetStockReportModel();
        //    return View("StockReport", stockReportModel);
        //}

        public ActionResult SendStockReportEmail()
        {
            var stockReportModel = GetStockReportModel();
            string reportHtml = RenderViewToString(this.ControllerContext, "StockReport", stockReportModel);
            SendEmail(reportHtml);

            return Json(new { success = true, message = "Email sent successfully!" }, JsonRequestBehavior.AllowGet);
        }

        private string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            context.Controller.ViewData.Model = model;
            using (var sw = new System.IO.StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(context, viewName, null);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private void SendEmail(string body)
        {
            string email = Convert.ToString(Session["Email"]);
            var fromAddress = new MailAddress("shraddhajadav635@gmail.com", "shraddha jadav");
            //var toAddress = new MailAddress(email);
            var toAddress = new MailAddress("apexaforedu201@gmail.com");

            string frompassword = "ezfh coto dlrz ojfw";
            const string subject = "Stock Report";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, frompassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
                TempData["EmailMsg"] = "Email Send Successfully!!";
            }
        }

    }
}




//Stored Procedure inside database

//create procedure getstockreport
//as
//begin
//select 

//p.name as productname,

//sum(case when s.type = 'in' then s.qty else 0 end) as totalstock,

//sum(case when s.type = 'in' then s.qty else 0 end) - sum(case when s.type = 'out' then s.qty else 0 end) - isnull(sum(o.quantity), 0) as remstock,

//case 
//when sum(case when s.type = 'in' then s.qty else 0 end) - sum(case when s.type = 'out' then s.qty else 0 end) - isnull(sum(o.quantity), 0) < p.threshold 
//then 'yes' 
//else 'no' 
//end as islowkey

//from 
//products p
//join 
//stocks s on p.productid = s.itemid
//left join 
//orderitems o on p.productid = o.itemid
//where 
//p.type = 'purchasable'
//group by 
//p.productid, p.name, p.threshold
//end



