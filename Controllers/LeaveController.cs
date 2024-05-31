using Task10.Models;
using Task10.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ExamTask.Controllers
{
    public class LeaveController : Controller
    {
        ExamEntities db = new ExamEntities();

        //Action te get all Leaves
        public ActionResult GetAbsenties()
        {

            var user = Session["UserID"];
            if (user != null)
            {
                var leave = db.Leaves.ToList();
                var approved = db.Leaves.Where(m => m.Status == "Approved").Select(m => m.FromDate).ToList();
                ViewBag.Approved = approved;
                return View("GetAbsenties", leave);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        //Action to approve and reject leave based on filters
        [HttpPost]
        public ActionResult ApproveLeave(int id, int leaveid, int userId, DateTime fromDate)
        {

            var type = db.Users.Where(m => m.UserId == userId).Select(m => m.Type).FirstOrDefault();

            var available = db.Leaves.Where(m => m.FromDate == fromDate && m.Status == "Approved").Select(m => m.UserId).ToList();
            var leave = db.Leaves.Where(m => m.LeaveId == leaveid).FirstOrDefault();
            if (available.Count != 0)
            {
                foreach (var item in available)
                {
                    if (db.Users.Where(m => m.UserId == item).Select(m => m.Type).FirstOrDefault() == type && db.Users.Where(m => m.UserId == item).Select(m => m.UserId).FirstOrDefault() != userId)
                    {
                        leave.Status = "Rejected";
                        db.SaveChanges();
                        return Content("Another user of same type is already on the leave on that day");
                    }
                    else
                    {

                        if (id == 1)
                        {
                            leave.Status = "Approved";
                            db.SaveChanges();
                        }
                        else
                        {
                            leave.Status = "Rejected";
                            db.SaveChanges();
                        }
                    }
                }
            }
            else
            {
                if (id == 1)
                {
                    leave.Status = "Approved";
                    db.SaveChanges();
                }
                else
                {
                    leave.Status = "Rejected";
                    db.SaveChanges();
                }
            }



            var leaves = db.Leaves.ToList();
            return View("GetAbsenties", leaves);

        }
    }
}