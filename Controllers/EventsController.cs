using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class EventsController : BaseController
    {
        private readonly Model1 db = new Model1();
        private const string BannerDirectory = "~/Content/img/events/";

        // GET: Events
        public ActionResult Index()
        {
            try
            {
                var events = db.tblEvents
                    .Include(e => e.tblEventHead)
                    .OrderByDescending(e => e.EventDate)
                    .ThenByDescending(e => e.EventID)
                    .ToList();

                // Prepare department title lookup (DepartmentID -> Title)
                var deptLookup = db.tblDepartments
                    .Select(d => new { d.DepartmentID, d.Title })
                    .ToDictionary(d => d.DepartmentID, d => d.Title);
                ViewBag.DepartmentTitles = deptLookup;

                return View(events);
            }
            catch (Exception)
            {
                ViewBag.DepartmentTitles = new System.Collections.Generic.Dictionary<int, string>();
                return View(new List<tblEvent>());
            }
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            PopulateDropdowns();
            return View(new tblEvent { EventDate = DateTime.Now, Date = DateTime.Now, Active = true });
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Date,EventDate,AlumniEvent,EventHeadID,DepartmentID,Venue,Title,Details,Active")] tblEvent model, HttpPostedFileBase bannerFile)
        {
            if (ModelState.IsValid)
            {
                HandleBannerUpload(model, bannerFile);
                model.Views = 0;
                db.tblEvents.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDropdowns(model);
            return View(model);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ev = db.tblEvents.Find(id);
            if (ev == null) return HttpNotFound();
            PopulateDropdowns(ev);
            return View(ev);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "EventID,Date,EventDate,AlumniEvent,EventHeadID,DepartmentID,Venue,Title,Banner,Details,Views,Active")] tblEvent model, HttpPostedFileBase bannerFile)
        {
            if (ModelState.IsValid)
            {
                HandleBannerUpload(model, bannerFile, replaceExisting: true);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateDropdowns(model);
            return View(model);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ev = db.tblEvents.Include(e => e.tblEventHead).FirstOrDefault(e => e.EventID == id);
            if (ev == null) return HttpNotFound();
            
            // Add department title to ViewBag
            if (ev.DepartmentID.HasValue)
            {
                var dept = db.tblDepartments.FirstOrDefault(d => d.DepartmentID == ev.DepartmentID.Value);
                ViewBag.DepartmentTitle = dept?.Title;
            }
            
            return View(ev);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var ev = db.tblEvents.Find(id);
                if (ev == null)
                {
                    if (Request.IsAjaxRequest())
                        return Json(new { success = false, message = "Event not found" });
                    return RedirectToAction("Index");
                }

                // Delete banner file
                if (!string.IsNullOrEmpty(ev.Banner))
                {
                    try
                    {
                        var path = Server.MapPath(BannerDirectory + ev.Banner);
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }
                    catch { /* ignore file errors */ }
                }

                db.tblEvents.Remove(ev);
                db.SaveChanges();

                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "Event deleted successfully" });
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Error deleting event: " + ex.Message });
                ModelState.AddModelError("", "Error deleting event: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        private void HandleBannerUpload(tblEvent model, HttpPostedFileBase bannerFile, bool replaceExisting = false)
        {
            if (bannerFile != null && bannerFile.ContentLength > 0)
            {
                var ext = Path.GetExtension(bannerFile.FileName)?.ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Banner", "Only image files (jpg, jpeg, png, gif, webp) are allowed.");
                    return;
                }

                // Ensure directory exists
                var dir = Server.MapPath(BannerDirectory);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // Delete old if replacing
                if (replaceExisting && !string.IsNullOrEmpty(model.Banner))
                {
                    try
                    {
                        var oldPath = Path.Combine(dir, model.Banner);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    catch { }
                }

                var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                var path = Path.Combine(dir, fileName);
                bannerFile.SaveAs(path);
                model.Banner = fileName;
            }
        }

        private void PopulateDropdowns(tblEvent ev = null)
        {
            try
            {
                var heads = db.tblEventHeads.OrderBy(h => h.C_Order).ThenBy(h => h.Title).ToList();
                ViewBag.EventHeads = new SelectList(heads, "EventHeadID", "Title", ev?.EventHeadID);

                var departments = db.tblDepartments.OrderBy(d => d.C_Order).ThenBy(d => d.Title).ToList();
                ViewBag.Departments = new SelectList(departments, "DepartmentID", "Title", ev?.DepartmentID);
            }
            catch
            {
                ViewBag.EventHeads = new SelectList(new List<SelectListItem>(), "Value", "Text");
                ViewBag.Departments = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
