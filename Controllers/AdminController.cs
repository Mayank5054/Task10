using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
        public ActionResult GetLeaveData()
        {
            var today = DateTime.Today;
            //var leaveData = _db.getLeaveData(d.ToString());
            //var a= _db.Leaves.Select(x => new {
            //    Name = x.User.Fullname
            //}).ToList(); 
            var leaveData = _db.Leaves.Where(y => y.FromDate <= today && y.ToDate >= today).Select(x => new
            {
                Name = x.User.Fullname
            }).ToList();

            return Json(new { data = leaveData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOvertimeData()
        {
            var today = DateTime.Today;
            var OverTimeData = _db.Overtimes.Where(y => y.FromDate <= today && y.ToDate >= today).Select(x => new {
                Name = x.User.Fullname
            }).ToList();

            return Json(new { data = OverTimeData }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAttendeeList(int id)
        {
            var AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook" && x.UserId != id).Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
            return Json(new { data = AttendeeList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetChildData(int id)
        {
            var AttendeeList = _db.CookingSessionAttendees.Where(x => x.SessionId == id && x.IsDeleted == 0).Select(y => new
            {
                name = y.User.Fullname,
                create = y.CreatedOn.ToString(),
                update = y.UpdatedOn.ToString(),
            }).ToList();
            return Json(new { data = AttendeeList }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CookingSession()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult GetCookingData()
        {
            var CookingData = _db.CookingSessions.Where(y => y.IsDeleted == 0 && y.User.Type.ToLower() == "cook").Select(x => new {
                Name = x.User.Fullname,
                date = x.SessionDate.ToString(),
                SessionId = x.SessionId
            }).ToList();

            return Json(new { data = CookingData }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRevenueData()
        {
            var today = DateTime.Now.Date;
            var RevenueData = _db.Orders
                                .Where(x => DbFunctions.TruncateTime(x.DateOfOrder) == today)
                                .Sum(order => order.TotalAmount);

            return Json(new { data = RevenueData }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetForm(int id)
        {
            if (id == 0)
            {

                CookingSession cs = new CookingSession();
                cs.TrainerList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                cs.AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                cs.AttendeeId = new List<string>();
                //cs.TrainerList = _db.CookingSessions.Where(x=>x.User.Type != "Customer").Select(y=>new SelectListItem { Text = y.User.Fullname,Value=y.User.UserId.ToString() }).ToList();
                return PartialView("_CookingSession", cs);
            }
            else
            {
                CookingSession cs = _db.CookingSessions.Find(id);
                CookingSessionAttendee ca = _db.CookingSessionAttendees.Where(x => x.SessionId == id).FirstOrDefault();
                cs.TrainerList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString(), Selected = (y.UserId == cs.TranierId) }).ToList();
                cs.AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem
                {
                    Text = y.Fullname,
                    Value = y.UserId.ToString(),
                    Selected = ca.AttendeeId.ToString().Contains(y.UserId.ToString())
                }).ToList();
                cs.AttendeeId = new List<string>();
                return PartialView("_CookingSession", cs);
            }
        }
        public ActionResult delete(int id)
        {
            var cs = _db.CookingSessions.Where(x => x.SessionId == id).FirstOrDefault();
            var ca = _db.CookingSessionAttendees.Where(x => x.SessionId == id).ToList();
            cs.IsDeleted = 1;
            for (int i = 0; i < ca.Count(); i++)
            {
                ca[i].IsDeleted = 1;
                _db.CookingSessionAttendees.AddOrUpdate(ca[i]);
            }
            cs.AttendeeId = new List<string> { "0" };
            _db.CookingSessions.AddOrUpdate(cs);
            _db.SaveChanges();
            return Json(new { success = "true" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult submitCookingData(CookingSession cs)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CookingSession c = new CookingSession();
                    c.SessionId = cs.SessionId;
                    c.SessionDate = cs.SessionDate;
                    c.TranierId = cs.TranierId;
                    var check = _db.Leaves.Where(x => x.UserId == cs.TranierId && x.FromDate <= cs.SessionDate && x.ToDate >= cs.SessionDate).FirstOrDefault();
                    if (check != null)
                    {
                        ModelState.AddModelError("AttendeeId", "Trainer is on leave this date please select another one");
                        cs.TrainerList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                        cs.AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                        return PartialView("_CookingSession", cs);
                    }
                    c.CreatedOn = DateTime.Now;
                    c.UpdatedOn = DateTime.Now;
                    c.CreatedBy = int.Parse(Session["UserId"].ToString());
                    c.IsDeleted = 0;

                    foreach (var attendeeId in cs.AttendeeId)
                    {
                        var AId = (int)Convert.ToUInt32(attendeeId);
                        var checkAttendee = _db.Leaves.Where(x => x.UserId == AId && x.FromDate <= cs.SessionDate && x.ToDate >= cs.SessionDate).FirstOrDefault();
                        if (checkAttendee != null)
                        {
                            var name = _db.Users.FirstOrDefault(x => x.UserId == AId).Fullname;
                            ModelState.AddModelError("AttendeeId", $"{name} is on leave this date please select another attendee");
                            cs.TrainerList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                            cs.AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                            return PartialView("_CookingSession", cs);
                        }
                    }

                    c.AttendeeId = new List<string>(cs.AttendeeId);
                    _db.CookingSessions.Add(c);
                    _db.SaveChanges();

                    int sessionid = c.SessionId;

                    using (var context = new ExamEntities())
                    {
                        CookingSessionAttendee ca = new CookingSessionAttendee();
                        foreach (var attendeeId in cs.AttendeeId)
                        {
                            ca.SessionId = sessionid;
                            ca.AttendeeId = (int)Convert.ToUInt32(attendeeId);
                            ca.CreatedOn = DateTime.Now;
                            ca.UpdatedOn = DateTime.Now;
                            ca.IsDeleted = 0;
                            var cookingSessionAttendee = _db.CookingSessionAttendees.Where(x => x.SessionId == ca.SessionId).ToList();
                            context.CookingSessionAttendees.Add(ca);
                            context.SaveChanges();
                        }
                    }
                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                return View();
            }
            catch
            {
                FormsAuthentication.SignOut();

                return Json(new { success = false, redirectUrl = Url.Action("Login", "Auth") });
            }
        }
        [HttpPost]
        public ActionResult EditCookingData(CookingSession cs, List<string> AttendeeId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CookingSession c = new CookingSession();
                    c.SessionId = cs.SessionId;
                    c.SessionDate = cs.SessionDate;
                    c.TranierId = cs.TranierId;
                    var check = _db.Leaves.Where(x => x.UserId == cs.TranierId && x.FromDate <= cs.SessionDate && x.ToDate >= cs.SessionDate).FirstOrDefault();
                    if (check != null)
                    {
                        ModelState.AddModelError("AttendeeId", "Trainer is on leave this date please select another one");
                        cs.TrainerList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                        cs.AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                        return PartialView("_CookingSession", cs);
                    }
                    c.UpdatedOn = DateTime.Now;
                    c.CreatedBy = int.Parse(Session["UserId"].ToString());
                    c.IsDeleted = 0;

                    foreach (var attendeeId in cs.AttendeeId)
                    {
                        var AId = (int)Convert.ToUInt32(attendeeId);
                        var checkAttendee = _db.Leaves.Where(x => x.UserId == AId && x.FromDate <= cs.SessionDate && x.ToDate >= cs.SessionDate).FirstOrDefault();
                        if (checkAttendee != null)
                        {
                            var name = _db.Users.FirstOrDefault(x => x.UserId == AId).Fullname;
                            ModelState.AddModelError("AttendeeId", $"{name} is on leave this date please select another attendee");
                            cs.TrainerList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                            cs.AttendeeList = _db.Users.Where(x => x.Type.ToLower() == "cook").Select(y => new SelectListItem { Text = y.Fullname, Value = y.UserId.ToString() }).ToList();
                            return PartialView("_CookingSession", cs);
                        }
                    }

                    CookingSession _cs = _db.CookingSessions.Where(x => x.SessionId == cs.SessionId).FirstOrDefault();
                    _cs.TranierId = cs.TranierId;
                    _cs.SessionDate = cs.SessionDate;
                    _cs.UpdatedOn = DateTime.Now;
                    _cs.AttendeeId = AttendeeId;

                    _db.SaveChanges();

                    int sessionid = c.SessionId;


                    CookingSessionAttendee ca = new CookingSessionAttendee();
                    var cookingSessionAttendee = _db.CookingSessionAttendees.Where(x => x.SessionId == sessionid).ToList();

                    // Step 1: Set IsDeleted to 1 for all old data entries
                    foreach (var attendee in cookingSessionAttendee)
                    {
                        attendee.IsDeleted = 1;
                    }

                    // Step 2: Process the updated data
                    foreach (var id in AttendeeId)
                    {
                        var aid = Convert.ToInt32(id);
                        var existingAttendee = cookingSessionAttendee.FirstOrDefault(x => x.AttendeeId == aid);

                        if (existingAttendee != null)
                        {
                            // Set IsDeleted to 0 if the attendee is found in the updated list
                            existingAttendee.IsDeleted = 0;
                            existingAttendee.UpdatedOn = DateTime.Now;
                        }
                        else
                        {
                            // Add new attendee if it's not in the old data
                            _db.CookingSessionAttendees.Add(new CookingSessionAttendee
                            {
                                SessionId = sessionid,
                                AttendeeId = aid,
                                IsDeleted = 0,
                                CreatedOn = DateTime.Now,
                                UpdatedOn = DateTime.Now
                            });
                        }
                    }


                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                return View();
            }
            catch
            {
                FormsAuthentication.SignOut();

                return Json(new { success = false, redirectUrl = Url.Action("Login", "Authentication") });
            }
        }
    }
    }
