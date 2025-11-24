using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.IO;
using System.Web;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class SuccessStoriesController : BaseController
    {
        private readonly Model1 db = new Model1();
        private readonly string uploadPath = "~/Content/img/departments/SuccessStories/";

        // GET: SuccessStories
        public ActionResult Index(string name, string registrationNo, string organization, string designation, int? departmentId, bool? active)
        {
            var query = db.tblSuccessStories.AsQueryable();

            // Filters
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(s => s.FullName.Contains(name));
            if (!string.IsNullOrWhiteSpace(registrationNo))
                query = query.Where(s => s.RegistrationNo.Contains(registrationNo));
            if (!string.IsNullOrWhiteSpace(organization))
                query = query.Where(s => s.Organization.Contains(organization));
            if (!string.IsNullOrWhiteSpace(designation))
                query = query.Where(s => s.Designation.Contains(designation));
            if (departmentId.HasValue)
                query = query.Where(s => s.DepartmentID == departmentId.Value);
            if (active.HasValue)
                query = query.Where(s => s.Active == active.Value);

            var list = query
                .OrderBy(s => s.FullName)
                .ToList();

            // ViewBag data for filters
            ViewBag.FilterName = name;
            ViewBag.FilterRegistrationNo = registrationNo;
            ViewBag.FilterOrganization = organization;
            ViewBag.FilterDesignation = designation;
            ViewBag.FilterDepartmentId = departmentId;
            ViewBag.FilterActive = active;
            ViewBag.Departments = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", departmentId);

            return View(list);
        }

        // GET: SuccessStories/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title");
            return View(new tblSuccessStory { Active = true });
        }

        // POST: SuccessStories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblSuccessStory model, HttpPostedFileBase photoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (photoFile != null && photoFile.ContentLength > 0)
                {
                    // Save the record first to get the ID
                    db.tblSuccessStories.Add(model);
                    db.SaveChanges();

                    // Now rename and save the file
                    string fileName = SavePhotoFile(photoFile, model.ID);
                    model.Photo = fileName;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.tblSuccessStories.Add(model);
                    db.SaveChanges();
                }

                TempData["Success"] = "Success Story added successfully.";
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            return View(model);
        }

        // GET: SuccessStories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var entity = db.tblSuccessStories.Find(id);
            if (entity == null)
                return HttpNotFound();

            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", entity.DepartmentID);
            return View(entity);
        }

        // POST: SuccessStories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblSuccessStory model, HttpPostedFileBase photoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload
                if (photoFile != null && photoFile.ContentLength > 0)
                {
                    // Delete old photo if exists
                    if (!string.IsNullOrEmpty(model.Photo))
                    {
                        DeletePhotoFile(model.Photo);
                    }

                    // Save new photo
                    string fileName = SavePhotoFile(photoFile, model.ID);
                    model.Photo = fileName;
                }

                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Success Story updated successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            return View(model);
        }

        // POST: SuccessStories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var entity = db.tblSuccessStories.Find(id);
            if (entity == null)
                return Json(new { success = false, message = "Entry not found." });

            // Delete associated photo file
            if (!string.IsNullOrEmpty(entity.Photo))
            {
                DeletePhotoFile(entity.Photo);
            }

            db.tblSuccessStories.Remove(entity);
            db.SaveChanges();
            return Json(new { success = true, message = "Success Story deleted successfully." });
        }

        // POST: SuccessStories/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            var entity = db.tblSuccessStories.Find(id);
            if (entity == null)
                return Json(new { success = false, message = "Entry not found." });

            entity.Active = !entity.Active;
            db.SaveChanges();

            return Json(new { success = true, active = entity.Active });
        }

        // Helper method to save photo file with proper naming
        private string SavePhotoFile(HttpPostedFileBase file, int recordId)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            // Ensure directory exists
            string serverPath = Server.MapPath(uploadPath);
            if (!Directory.Exists(serverPath))
            {
                Directory.CreateDirectory(serverPath);
            }

            // Generate new filename: successStoryPhoto{ID}
            string extension = Path.GetExtension(file.FileName);
            string newFileName = $"successStoryPhoto{recordId}{extension}";

            // Save file
            string fullPath = Path.Combine(serverPath, newFileName);
            file.SaveAs(fullPath);

            return newFileName;
        }

        // Helper method to delete photo file
        private void DeletePhotoFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;

            string serverPath = Server.MapPath(uploadPath);
            string fullPath = Path.Combine(serverPath, fileName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
