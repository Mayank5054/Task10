using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Models;
namespace Task10.Controllers
{
    public class EventController : Controller
    {
        private readonly ExamEntities db;

        public EventController()
        {
            db = new ExamEntities();
        }
        public ActionResult Index()
        {
            int UserId = Convert.ToInt32(Session["UserId"]);
            List<Event> events = db.Events.Where(e => e.UserId == UserId && e.IsDeleted == 0).ToList();
            return View(events);
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            Event e = db.Events.Find(id);

            if (e == null)
            {
                ViewBag.Error = "No event found.";
                return HttpNotFound();
            }
            return View(e);
        }

        public ActionResult CreateEventItem()
        {
            return View();
        }

        public ActionResult EventForm()
        {
            return PartialView("_CreateEvent");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Event e)
        {
            if (!ModelState.IsValid)
            {
                return View(e);
            }
            // ?if there is no event on same date
            if (!db.Events.Any(ev => ev.EventDate == e.EventDate && ev.IsDeleted == 0))
            {
                e.CreatedOn = DateTime.Now;
                e.UpdatedOn = DateTime.Now;
                e.CreatedBy = Convert.ToInt32(Session["UserId"]);
                e.UserId = Convert.ToInt32(Session["UserId"]);
                e.IsDeleted = 0;

                db.Events.Add(e);
                db.SaveChanges();
                return RedirectToAction("Index", "Event");
            }
            else
            {
                switch (e.Duration.ToLower())
                {
                    case "firsthalf":

                        // ?if there is any event on same duration (firsthalf | fullhalf)
                        if (db.Events.Any(ev => ev.EventDate == e.EventDate &&
                        (ev.Duration == "firsthalf" || ev.Duration == "fullhalf")
                        && ev.IsDeleted == 0))
                        {
                            ViewBag.Error = "You already have an event.";
                            return View(e);
                        }
                        else
                        {
                            e.CreatedOn = DateTime.Now;
                            e.UpdatedOn = DateTime.Now;
                            e.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            e.UserId = Convert.ToInt32(Session["UserId"]);
                            e.IsDeleted = 0;

                            db.Events.Add(e);
                            db.SaveChanges();
                            return RedirectToAction("Index", "Event");
                        }
                    case "secondhalf":

                        // ?if there is any event on same duration (secondhalf | fullhalf)
                        if (db.Events.Any(ev => ev.EventDate == e.EventDate &&
                        (ev.Duration == "secondhalf" || ev.Duration == "fullhalf") &&
                        ev.IsDeleted == 0))
                        {
                            ViewBag.Error = "You already have an event.";
                            return View(e);
                        }
                        else
                        {
                            e.CreatedOn = DateTime.Now;
                            e.UpdatedOn = DateTime.Now;
                            e.CreatedBy = Convert.ToInt32(Session["UserId"]);
                            e.UserId = Convert.ToInt32(Session["UserId"]);
                            e.IsDeleted = 0;

                            db.Events.Add(e);
                            db.SaveChanges();
                            return RedirectToAction("Index", "Event");
                        }

                    // ?dont allow fullhalf
                    default:
                        ViewBag.Error = "You already have an event.";
                        return View(e);

                }
            }

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Event e)
        {
            if (!ModelState.IsValid)
            {
                return View(e);
            }
            Event existingEvent = db.Events.Find(e.EventId);
            // ?if there is no event on same date
            if (!db.Events.Any(ev => ev.EventDate == e.EventDate && ev.IsDeleted == 0))
            {

                existingEvent.UpdatedOn = DateTime.Now;
                existingEvent.QtyOfDishes = e.QtyOfDishes;
                existingEvent.Duration = e.Duration;
                existingEvent.EventDate = e.EventDate;

                db.SaveChanges();
                return RedirectToAction("Index", "Event");
            }
            else
            {
                switch (e.Duration.ToLower())
                {
                    // ?if there is any event on same duration (firsthalf | fullhalf)
                    case "firsthalf":
                        if (db.Events.Any(ev => ev.EventId != e.EventId &&
                        ev.EventDate == e.EventDate &&
                        (ev.Duration == "firsthalf" || ev.Duration == "fullhalf") &&
                        ev.IsDeleted == 0))
                        {
                            ViewBag.Error = "You already have an event.";
                            return View(e);
                        }
                        else
                        {

                            existingEvent.UpdatedOn = DateTime.Now;
                            existingEvent.QtyOfDishes = e.QtyOfDishes;
                            existingEvent.Duration = e.Duration;
                            existingEvent.EventDate = e.EventDate;

                            db.SaveChanges();
                            return RedirectToAction("Index", "Event");
                        }

                    // ?if there is any event on same duration (secondhalf | fullhalf)
                    case "secondhalf":
                        if (db.Events.Any(ev => ev.EventId != e.EventId &&
                        ev.EventDate == e.EventDate &&
                        (ev.Duration == "secondhalf" || ev.Duration == "fullhalf") &&
                        ev.IsDeleted == 0))
                        {
                            ViewBag.Error = "You already have an event.";
                            return View(e);
                        }
                        else
                        {

                            existingEvent.UpdatedOn = DateTime.Now;
                            existingEvent.QtyOfDishes = e.QtyOfDishes;
                            existingEvent.Duration = e.Duration;
                            existingEvent.EventDate = e.EventDate;

                            db.SaveChanges();
                            return RedirectToAction("Index", "Event");
                        }

                    // ?if there is any event on same duration (secondhalf | firsthalf)
                    default:

                        if (db.Events.Any(ev => ev.EventId != e.EventId &&
                        ev.EventDate == e.EventDate &&
                        (ev.Duration == "secondhalf" || ev.Duration == "firsthalf") &&
                        ev.IsDeleted == 0))
                        {
                            ViewBag.Error = "You already have an event.";
                            return View(e);
                        }

                        existingEvent.UpdatedOn = DateTime.Now;
                        existingEvent.QtyOfDishes = e.QtyOfDishes;
                        existingEvent.Duration = e.Duration;
                        existingEvent.EventDate = e.EventDate;

                        db.SaveChanges();
                        return RedirectToAction("Index", "Event");

                }
            }

        }


        public ActionResult Delete(int id)
        {
            Event e = db.Events.Find(id);
            if (e == null)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                //db.Events.Remove(e);
                //db.SaveChanges();

                //! delete corresponding eventitems
                List<EventItem> eventItems = db.EventItems.Where(ei => ei.EventId == id && ei.IsDeleted == 0).ToList();

                eventItems.ForEach(item => item.IsDeleted = 1);

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