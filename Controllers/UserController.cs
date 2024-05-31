using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Task10.Models;

namespace Task10.Controllers
{
    public class UserController : Controller
    {
        ExamEntities db = new ExamEntities();
        public ActionResult Index()
        {
            var id = (int)Session["UserId"];
            var availabe = db.DineInTables.Where(x => x.CanBeReserved == true).Select(x => x.TableId).ToList();


            List<object> ReservedData = new List<object>();


            foreach (var i in availabe)
            {
                var datac = db.ReservedTables.Where(x => x.TableId == i && x.UserId == id).FirstOrDefault();
                if (datac != null)
                {
                    ReservedData.Add(new
                    {
                        from = datac.FromDateTime,
                        to = datac.ToDateTime,
                        TableId = datac.TableId,
                        id = datac.ReservedTableId,
                        userID = datac.UserId,
                        total = datac.NoOFPeople,
                    });
                }

            }
            return View(ReservedData);
        }


        public ActionResult BookTable()
        {

            return View();
        }

        public ActionResult MyBookTable()
        {
            int id = (int)Session["UserId"];
            var data = db.ReservedTables.Where(x => x.UserId == id).ToList();
            return View(data);
        }

        public ActionResult GetReservedTable()
        {
            //var data = db.ReservedTables.Select(x => new
            //{
            //    x.TableId,
            //    fromHour = x.FromDateTime.HasValue ? (x.FromDateTime.Value.Hour).ToString():null,
            //    toHour = x.ToDateTime.HasValue ? (x.ToDateTime.Value.Hour).ToString():null,
            //    Frommin = x.FromDateTime.HasValue ? (x.FromDateTime.Value.Minute).ToString():null,
            //    Tomin = x.ToDateTime.HasValue ? (x.ToDateTime.Value.Minute).ToString():null,
            //}).ToList();  

            var data = db.ReservedTables.Select(x => new
            {
                x.TableId,
                x.FromDateTime,
                x.ToDateTime,
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BookTheTable(int id, string from, string to, int total)
        {

            DateTime fromdateTime = DateTime.ParseExact(from, "dd/MM/yyyyTHH:mm", CultureInfo.InvariantCulture);
            DateTime todateTime = DateTime.ParseExact(to, "dd/MM/yyyyTHH:mm", CultureInfo.InvariantCulture);


            var data = new ReservedTable();
            data.NoOFPeople = total;
            data.TableId = id;
            data.FromDateTime = fromdateTime;
            data.ToDateTime = todateTime;
            data.UserId = (int)Session["UserId"];


            db.ReservedTables.Add(data);
            db.SaveChanges();



            return RedirectToAction("MyBookTable", "User");

        }

        //public ActionResult BookNewTable(ReservedTable data)
        //{

        //    var availabeTable = db.DineInTables.Where(x => x.CanBeReserved == true && x.NoOFChairs >= data.NoOFPeople).ToList();
        //    return View("AvailableTable", availabeTable);
        //} 

        public ActionResult BookNewTable(string from, string to, int total, DateTime bookDate)
        {
            //var availabeTable = db.DineInTables.Where(x => x.CanBeReserved == true && x.NoOFChairs >= total).Select(x => new
            //{
            //    x.TableId,
            //    x.NoOFChairs,

            //}).ToList();

            //var availabeTable = db.DineInTables.Where(x => x.NoOFChairs >= total).Select(x => new
            //{
            //    x.TableId,
            //    x.NoOFChairs,
            //    x.CanBeReserved,

            //}).ToList();
            string dateString = bookDate.ToString("yyyy-MM-dd");

            string fromDate;
            string toDate;
            string format = "yyyy-MM-dd HH:mm:ss";

            string pattern = @"(\d+):(\d+)";

            // Match the pattern against the time string
            Match match = Regex.Match(from, pattern);
            Match match2 = Regex.Match(to, pattern);

            if (match.Success && match2.Success)
            {
                // Extract the hours and minutes as integers
                int hours = int.Parse(match.Groups[1].Value);
                int minutes = int.Parse(match.Groups[2].Value);

                // Construct fromDate in the format "yyyy-MM-dd HH:mm:ss"
                fromDate = $"{dateString} {hours:00}:{minutes:00}:00";

                // Extract the hours and minutes as integers
                int hours1 = int.Parse(match2.Groups[1].Value);
                int minutes2 = int.Parse(match2.Groups[2].Value);

                // Construct toDate in the format "yyyy-MM-dd HH:mm:ss"
                toDate = $"{dateString} {hours1:00}:{minutes2:00}:00";

                DateTime FromdateTime;
                DateTime TodateTime;

                // Parse fromDate and toDate strings into DateTime objects
                if (DateTime.TryParseExact(fromDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out FromdateTime) &&
                    DateTime.TryParseExact(toDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out TodateTime))
                {
                    // Call your method to get available tables based on FromdateTime, TodateTime, and total
                    var availableTables = db.new_get_notReserved_Table_forBooking(FromdateTime, TodateTime, total);
                    return Json(new { data = availableTables }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // Handle parsing errors
                    return Json(new { error = "Error parsing date and time" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                // Handle invalid time format
                return Json(new { error = "Invalid time format" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}