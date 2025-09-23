using System;
using System.Linq;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class DownloadsController : Controller
    {
        private readonly Model1 db = new Model1();

        [HttpGet]
        public ActionResult Index()
        {
            var model = db.tblDownloadsNews.FirstOrDefault();
            if (model == null)
            {
                // Create the initial row automatically with timestamps
                var created = new tblDownloadsNew
                {
                    Id = GetNextId(),
                    PageTitle = "Downloads",
                    PageData = string.Empty,
                    UploadedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                db.tblDownloadsNews.Add(created);
                db.SaveChanges();
                model = created;
            }
            ViewBag.Title = "Downloads";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(tblDownloadsNew input)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Downloads";
                return View(input);
            }

            if (input.Id > 0)
            {
                var entity = db.tblDownloadsNews.Find(input.Id);
                if (entity == null) return HttpNotFound();
                entity.PageTitle = input.PageTitle;
                entity.PageData = input.PageData;
                entity.ModifiedDate = DateTime.Now;
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var newEntity = new tblDownloadsNew
                {
                    Id = GetNextId(),
                    PageTitle = input.PageTitle,
                    PageData = input.PageData,
                    UploadedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
                db.tblDownloadsNews.Add(newEntity);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private int GetNextId()
        {
            return db.tblDownloadsNews.Any() ? db.tblDownloadsNews.Max(x => x.Id) + 1 : 1;
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
