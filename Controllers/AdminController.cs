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

        public ActionResult AddUser()
        {
            var departments = new List<SelectListItem>
            {
                new SelectListItem { Text = "cook", Value = "cook" },
                new SelectListItem { Text = "cashier", Value = "cashier" }

            };
            ViewBag.Departments = departments;
            var users = _db.Users.Select(m => m.Email).ToList();
            ViewBag.Users = users;
            return View("AddUser");
        }
        [HttpPost]
        public ActionResult AddUser(User model)
        {
            _db.Users.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }

        //Action to get all cooks
        public ActionResult GetCook()
        {
            var user = Session["UserId"];
            if (user != null)
            {
                var users = _db.Users.Where(m => m.Type == "cook" && m.Isdeleted != 1).ToList();
                return View("GetCook", users);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

        }

        //Action to edit cooks and cashier
        public ActionResult EditCook(int id)
        {

            var user = Session["UserId"];
            if (user != null)
            {
                var users = _db.Users.Where(m => m.UserId == id).FirstOrDefault();
                var departments = new List<SelectListItem>
                {
                    new SelectListItem { Text = "cook", Value = "cook" },
                    new SelectListItem { Text = "cashier", Value = "cashier" }

                };
                ViewBag.Departments = departments;
                return View("EditCook", users);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }


        }
        [HttpPost]
        public ActionResult EditCook(User model)
        {
            var user = Session["UserId"];
            if (user != null)
            {
                User users = _db.Users.Where(m => m.UserId == model.UserId).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    users.Fullname = model.Fullname;
                    users.Type = model.Type;
                    users.ContactNo = model.ContactNo;
            
                    _db.SaveChanges();
                }
                return Json(new { status="success"});
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

        //Action to delete cooks and cashier
        public ActionResult DeleteCook(int id)
        {
            var user = _db.Users.Where(m => m.UserId == id).FirstOrDefault();
            user.Isdeleted = 1;
            _db.SaveChanges();
            return RedirectToAction("Index", "Admin");
        }


        //Action to get all cashiers
        public ActionResult GetCashier()
        {
            var user = Session["UserId"];
            if (user != null)
            {
                var users = _db.Users.Where(m => m.Type == "cashier" && m.Isdeleted == 0).ToList();

                return View("GetCashier", users);
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }
        }

    }
    }
