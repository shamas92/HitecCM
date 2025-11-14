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
    public class EventPhotosController : BaseController
    {
        private readonly Model1 db = new Model1();
        private const string PhotoDirectory = "~/Content/img/eventphotos/";
        private const int PageSize = 12; // Number of items per page

        // GET: EventPhotos
        public ActionResult Index(int? page, string search, int? eventFilter, string statusFilter)
        {
            try
            {
                var query = db.tblEventPhotos
                    .Include(p => p.tblEvent)
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(p => p.Title.Contains(search) || 
                                           p.tblEvent.Title.Contains(search));
                }

                // Apply event filter
                if (eventFilter.HasValue && eventFilter.Value > 0)
                {
                    query = query.Where(p => p.EventID == eventFilter.Value);
                }

                // Apply status filter
                if (!string.IsNullOrWhiteSpace(statusFilter))
                {
                    query = query.Where(p => p.Status == statusFilter);
                }

                // Get total count before pagination
                var totalCount = query.Count();

                // Calculate pagination
                var currentPage = page ?? 1;
                var totalPages = (int)Math.Ceiling((double)totalCount / PageSize);

                // Ensure current page is valid
                if (currentPage < 1) currentPage = 1;
                if (currentPage > totalPages && totalPages > 0) currentPage = totalPages;

                // Get paginated data
                var photos = query
                    .OrderByDescending(p => p.Date)
                    .ThenByDescending(p => p.EventPhotoID)
                    .Skip((currentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();

                // Pass pagination info to view
                ViewBag.CurrentPage = currentPage;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalCount = totalCount;
                ViewBag.PageSize = PageSize;
                ViewBag.Search = search;
                ViewBag.EventFilter = eventFilter;
                ViewBag.StatusFilter = statusFilter;

                // Populate event dropdown for filter
                PopulateEventFilterDropdown(eventFilter);

                return View(photos);
            }
            catch (Exception)
            {
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 0;
                ViewBag.TotalCount = 0;
                return View(new List<tblEventPhoto>());
            }
        }

        // GET: EventPhotos/Create
        public ActionResult Create()
        {
            PopulateEventDropdown();
            return View(new tblEventPhoto { Date = DateTime.Now, Status = "Active" });
        }

        // POST: EventPhotos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,Date,Title,Status")] tblEventPhoto model, HttpPostedFileBase photoFile)
        {
            if (ModelState.IsValid)
            {
                HandlePhotoUpload(model, photoFile);
                
                if (string.IsNullOrEmpty(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Please select a photo to upload.");
                    PopulateEventDropdown(model);
                    return View(model);
                }

                db.tblEventPhotos.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateEventDropdown(model);
            return View(model);
        }

        // GET: EventPhotos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var photo = db.tblEventPhotos
                .Include(p => p.tblEvent)
                .FirstOrDefault(p => p.EventPhotoID == id);
            
            if (photo == null) return HttpNotFound();
            
            PopulateEventDropdown(photo);
            return View(photo);
        }

        // POST: EventPhotos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventPhotoID,EventID,Date,Photo,Title,Status")] tblEventPhoto model, HttpPostedFileBase photoFile)
        {
            if (ModelState.IsValid)
            {
                HandlePhotoUpload(model, photoFile, replaceExisting: true);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateEventDropdown(model);
            return View(model);
        }

        // GET: EventPhotos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var photo = db.tblEventPhotos
                .Include(p => p.tblEvent)
                .FirstOrDefault(p => p.EventPhotoID == id);
            
            if (photo == null) return HttpNotFound();
            
            return View(photo);
        }

        // POST: EventPhotos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var photo = db.tblEventPhotos.Find(id);
                if (photo == null)
                {
                    if (Request.IsAjaxRequest())
                        return Json(new { success = false, message = "Photo not found" });
                    return RedirectToAction("Index");
                }

                // Delete photo file
                if (!string.IsNullOrEmpty(photo.Photo))
                {
                    try
                    {
                        var path = Server.MapPath(PhotoDirectory + photo.Photo);
                        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                    }
                    catch { /* ignore file errors */ }
                }

                db.tblEventPhotos.Remove(photo);
                db.SaveChanges();

                if (Request.IsAjaxRequest())
                    return Json(new { success = true, message = "Photo deleted successfully" });
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                    return Json(new { success = false, message = "Error deleting photo: " + ex.Message });
                ModelState.AddModelError("", "Error deleting photo: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        private void HandlePhotoUpload(tblEventPhoto model, HttpPostedFileBase photoFile, bool replaceExisting = false)
        {
            if (photoFile != null && photoFile.ContentLength > 0)
            {
                var ext = Path.GetExtension(photoFile.FileName)?.ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Photo", "Only image files (jpg, jpeg, png, gif, webp) are allowed.");
                    return;
                }

                // Ensure directory exists
                var dir = Server.MapPath(PhotoDirectory);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // Delete old if replacing
                if (replaceExisting && !string.IsNullOrEmpty(model.Photo))
                {
                    try
                    {
                        var oldPath = Path.Combine(dir, model.Photo);
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }
                    catch { }
                }

                var fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + ext;
                var path = Path.Combine(dir, fileName);
                photoFile.SaveAs(path);
                model.Photo = fileName;
            }
        }

        private void PopulateEventDropdown(tblEventPhoto photo = null)
        {
            try
            {
                var events = db.tblEvents
                    .Where(e => e.Active == true || e.Active == null)
                    .OrderByDescending(e => e.EventDate)
                    .ThenBy(e => e.Title)
                    .ToList();
                
                ViewBag.Events = new SelectList(events, "EventID", "Title", photo?.EventID);
            }
            catch
            {
                ViewBag.Events = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
        }

        private void PopulateEventFilterDropdown(int? selectedEventId)
        {
            try
            {
                var events = db.tblEvents
                    .OrderByDescending(e => e.EventDate)
                    .ThenBy(e => e.Title)
                    .ToList();
                
                ViewBag.EventFilterList = new SelectList(events, "EventID", "Title", selectedEventId);
            }
            catch
            {
                ViewBag.EventFilterList = new SelectList(new List<SelectListItem>(), "Value", "Text");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
