using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Models;

namespace Task10.Controllers
{

        // GET: Admin
        public class AdminController : Controller
        {
            // GET: Admin
            ExamEntities _db = new ExamEntities();

            public ActionResult Index()
            {
                return View();
            }

            public ActionResult OrderRefund()
            {
                return View();
            }


            public ActionResult Details()
            {



                string role = (string)Session["Type"];

                var data = _db.Orders.ToList();
                string email = Convert.ToString(Session["Email"]);
                User u = _db.Users.FirstOrDefault(x => x.Email == email);


                List<sp_orderDetails_Result> details = _db.sp_orderDetails(null).ToList();

                var totalRecord = details.Count();
                int recFilter = details.Count;
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DateFormatString = "dd/MM/yyyy"
                };
                return Json(details, JsonRequestBehavior.AllowGet);


            }


            public ActionResult ChangeStatus(string status, string id)
            {
                int deid;
                int.TryParse(id, out deid);
                OrderRefund or = _db.OrderRefunds.FirstOrDefault(x => x.OrderRefundId == deid);
                if (status == "rejected")
                {
                    or.Status = status;
                    or.Amount = 0;
                    or.Percentage = 0;
                    or.CreatedOn = null;

                }
                else
                {
                    or.Status = status;
                }



            _db.OrderRefunds.AddOrUpdate(or);
                _db.SaveChanges();


                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }

            public ActionResult RefundAmount(int refundId, decimal refundAmount)
            {

                OrderRefund orderRefund = _db.OrderRefunds.FirstOrDefault(x => x.OrderRefundId == refundId);
                Order order = _db.Orders.FirstOrDefault(x => x.OrderId == orderRefund.OrderId);
                if (orderRefund != null)
                {

                    orderRefund.Amount = refundAmount;
                    orderRefund.Percentage = (refundAmount * 100) / order.TotalAmount;
                    orderRefund.RefundDate = DateTime.Now;
                    _db.OrderRefunds.AddOrUpdate(orderRefund);
                    _db.SaveChanges();
                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        public ActionResult Dashbord()
        {
            List<Product> products = _db.Products.Where(p => p.Isdeleted == 0).ToList();
            return View(products);
        }
    }
    }
