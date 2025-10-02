using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;
using System.Data.Entity;

namespace HitecCM.Controllers
{
    public class HRController : Controller
    {
        private readonly Model1 db = new Model1();

        // Classification Services Action
        [HttpGet]
        public ActionResult ClassificationServices()
        {
            var model = db.tblHrClassificationServices.FirstOrDefault();
            if (model == null)
            {
                var created = new tblHrClassificationServices
                {
                    ID = GetNextClassificationServicesId(),
                    Title = "Classification Services",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrClassificationServices.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Classification Services";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ClassificationServices(tblHrClassificationServices input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Classification Services";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHrClassificationServices.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHrClassificationServices
                {
                    ID = GetNextClassificationServicesId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrClassificationServices.Add(newEntity);
            }

            db.SaveChanges();
            TempData["Success"] = "Classification Services updated successfully.";
            return RedirectToAction("ClassificationServices");
        }

        // ESTR Action
        [HttpGet]
        public ActionResult ESTR()
        {
            var model = db.tblHrESTRs.FirstOrDefault();
            if (model == null)
            {
                var created = new tblHrESTR
                {
                    ID = GetNextESTRId(),
                    Title = "ESTR (Employee Service Transfer Record)",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrESTRs.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "ESTR";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ESTR(tblHrESTR input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "ESTR";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHrESTRs.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHrESTR
                {
                    ID = GetNextESTRId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrESTRs.Add(newEntity);
            }

            db.SaveChanges();
            TempData["Success"] = "ESTR updated successfully.";
            return RedirectToAction("ESTR");
        }

        // Perks Action
        [HttpGet]
        public ActionResult Perks()
        {
            var model = db.tblHrPerks.FirstOrDefault();
            if (model == null)
            {
                var created = new tblHrPerks
                {
                    ID = GetNextPerksId(),
                    Title = "Employee Perks & Benefits",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrPerks.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Perks";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Perks(tblHrPerks input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Perks";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHrPerks.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHrPerks
                {
                    ID = GetNextPerksId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrPerks.Add(newEntity);
            }

            db.SaveChanges();
            TempData["Success"] = "Perks updated successfully.";
            return RedirectToAction("Perks");
        }

        // Leaves Action
        [HttpGet]
        public ActionResult Leaves()
        {
            var model = db.tblHrLeaves.FirstOrDefault();
            if (model == null)
            {
                var created = new tblHrLeaves
                {
                    ID = GetNextLeavesId(),
                    Title = "Leave Policies & Management",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrLeaves.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Leaves";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Leaves(tblHrLeaves input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Leaves";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHrLeaves.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHrLeaves
                {
                    ID = GetNextLeavesId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrLeaves.Add(newEntity);
            }

            db.SaveChanges();
            TempData["Success"] = "Leaves updated successfully.";
            return RedirectToAction("Leaves");
        }

        // Contributory Fund Action
        [HttpGet]
        public ActionResult ContributoryFund()
        {
            var model = db.tblHrContributoryFunds.FirstOrDefault();
            if (model == null)
            {
                var created = new tblHrContributoryFund
                {
                    ID = GetNextContributoryFundId(),
                    Title = "Contributory Fund Management",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrContributoryFunds.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Contributory Fund";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ContributoryFund(tblHrContributoryFund input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Contributory Fund";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHrContributoryFunds.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHrContributoryFund
                {
                    ID = GetNextContributoryFundId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrContributoryFunds.Add(newEntity);
            }

            db.SaveChanges();
            TempData["Success"] = "Contributory Fund updated successfully.";
            return RedirectToAction("ContributoryFund");
        }

        // Discipline Action
        [HttpGet]
        public ActionResult Discipline()
        {
            var model = db.tblHrDisciplines.FirstOrDefault();
            if (model == null)
            {
                var created = new tblHrDiscipline
                {
                    ID = GetNextDisciplineId(),
                    Title = "Discipline Policies & Procedures",
                    PageData = string.Empty,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrDisciplines.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Discipline";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Discipline(tblHrDiscipline input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Discipline";
                return View(input);
            }

            if (input.ID > 0)
            {
                var entity = db.tblHrDisciplines.Find(input.ID);
                if (entity == null) return HttpNotFound();
                entity.Title = input.Title;
                entity.PageData = input.PageData;
                entity.UpdatedAt = DateTime.Now;
                db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                var newEntity = new tblHrDiscipline
                {
                    ID = GetNextDisciplineId(),
                    Title = input.Title,
                    PageData = input.PageData,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.tblHrDisciplines.Add(newEntity);
            }

            db.SaveChanges();
            TempData["Success"] = "Discipline updated successfully.";
            return RedirectToAction("Discipline");
        }

        // Helper methods to get next IDs
        private int GetNextClassificationServicesId()
        {
            return db.tblHrClassificationServices.Any() ? db.tblHrClassificationServices.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextESTRId()
        {
            return db.tblHrESTRs.Any() ? db.tblHrESTRs.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextPerksId()
        {
            return db.tblHrPerks.Any() ? db.tblHrPerks.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextLeavesId()
        {
            return db.tblHrLeaves.Any() ? db.tblHrLeaves.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextContributoryFundId()
        {
            return db.tblHrContributoryFunds.Any() ? db.tblHrContributoryFunds.Max(x => x.ID) + 1 : 1;
        }

        private int GetNextDisciplineId()
        {
            return db.tblHrDisciplines.Any() ? db.tblHrDisciplines.Max(x => x.ID) + 1 : 1;
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