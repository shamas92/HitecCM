using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class FacilitiesController : BaseController
    {
        private readonly Model1 db = new Model1();

        // Transport Action - using the new tblTransport model
        [HttpGet]
        public ActionResult Transport()
        {
            var model = db.tblTransports.FirstOrDefault();
            if (model == null)
            {
                // Create the initial row automatically with timestamps
                var created = new tblTransport
                {
                    ID = GetNextTransportId(),
                    Title = "Transport",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblTransports.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Transport Facilities";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Transport(tblTransport input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Transport Facilities";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblTransports.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var newEntity = new tblTransport
                {
                    ID = GetNextTransportId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblTransports.Add(newEntity);
            }

            db.SaveChanges();
            return RedirectToAction("Transport");
        }

        // Sports Action
        [HttpGet]
        public ActionResult Sports()
        {
            var model = db.tblSports.FirstOrDefault();
            if (model == null)
            {
                // Create the initial row automatically with timestamps
                var created = new tblSports
                {
                    ID = GetNextSportsId(),
                    Title = "Sports",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblSports.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Sports Facilities";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Sports(tblSports input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Sports Facilities";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblSports.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var newEntity = new tblSports
                {
                    ID = GetNextSportsId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblSports.Add(newEntity);
            }

            db.SaveChanges();
            return RedirectToAction("Sports");
        }

        // Hostel Action
        [HttpGet]
        public ActionResult Hostel()
        {
            var model = db.tblHostels.FirstOrDefault();
            if (model == null)
            {
                // Create the initial row automatically with timestamps
                var created = new tblHostel
                {
                    ID = GetNextHostelId(),
                    Title = "Hostel",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHostels.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Hostel Facilities";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Hostel(tblHostel input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Hostel Facilities";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHostels.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHostel
                {
                    ID = GetNextHostelId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHostels.Add(newEntity);
            }

            db.SaveChanges();
            return RedirectToAction("Hostel");
        }

        // Cafeteria Action
        [HttpGet]
        public ActionResult Cafeteria()
        {
            var model = db.tblCafeterias.FirstOrDefault();
            if (model == null)
            {
                // Create the initial row automatically with timestamps
                var created = new tblCafeteria
                {
                    ID = GetNextCafeteriaId(),
                    Title = "Cafeteria",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblCafeterias.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Cafeteria Facilities";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Cafeteria(tblCafeteria input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Cafeteria Facilities";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblCafeterias.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var newEntity = new tblCafeteria
                {
                    ID = GetNextCafeteriaId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblCafeterias.Add(newEntity);
            }

            db.SaveChanges();
            return RedirectToAction("Cafeteria");
        }

        // Helper methods to get next IDs
        private int GetNextTransportId()
        {
            return db.tblTransports.Any() ? db.tblTransports.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextSportsId()
        {
            return db.tblSports.Any() ? db.tblSports.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextHostelId()
        {
            return db.tblHostels.Any() ? db.tblHostels.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextCafeteriaId()
        {
            return db.tblCafeterias.Any() ? db.tblCafeterias.Max(x => x.ID) + 1 : 1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}