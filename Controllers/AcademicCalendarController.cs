using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class AcademicCalendarController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new Model1())
            {
                var calendar = db.AcademicCalendars.FirstOrDefault();
                if (calendar == null)
                {
                    calendar = new AcademicCalendar();
                }
                return View(calendar);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(AcademicCalendar model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Model1())
                {
                    var calendar = db.AcademicCalendars.FirstOrDefault();
                    if (calendar == null)
                    {
                        calendar = new AcademicCalendar
                        {
                            CreatedAt = DateTime.Now
                        };
                        db.AcademicCalendars.Add(calendar);
                    }
                    calendar.Title = model.Title;
                    calendar.DescripQon = model.DescripQon;
                    calendar.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                }
                TempData["Success"] = "Academic Calendar updated successfully.";
                return RedirectToAction("Index");
            }
            using (var db = new Model1())
            {
                var calendar = db.AcademicCalendars.FirstOrDefault() ?? new AcademicCalendar();
                return View(calendar);
            }
        }
    }
} 