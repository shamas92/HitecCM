using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;

namespace HitecCM.Controllers
{
    public class FacultyInformationController : BaseController
    {
        private readonly Model1 db = new Model1();
        private const string FacultyImageVirtualDir = "~/Content/img/faculty"; // image directory

        // GET: FacultyInformation
        public ActionResult Index(string name, int? departmentId, int? designationId, bool? active, bool? studyLeave)
        {
            try
            {
                var query = db.tblFaculties
                    .Include(f => f.tblDepartment)
                    .Include(f => f.tblDesignation)
                    .AsQueryable();

                // Filters
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(f => f.FullName.Contains(name));
                }
                if (departmentId.HasValue)
                {
                    query = query.Where(f => f.DepartmentID == departmentId.Value);
                }
                if (designationId.HasValue)
                {
                    query = query.Where(f => f.DesignationID == designationId.Value);
                }
                if (active.HasValue)
                {
                    query = query.Where(f => f.Active == active.Value);
                }
                if (studyLeave.HasValue)
                {
                    query = query.Where(f => f.StudyLeave == studyLeave.Value);
                }

                var faculties = query
                    .OrderBy(f => f.OrderNo)
                    .ThenBy(f => f.FullName)
                    .ToList();

                // Dropdown data for filters
                ViewBag.FilterDepartments = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", departmentId);
                ViewBag.FilterDesignations = new SelectList(db.tblDesignations.OrderBy(d => d.Designation), "DesignationID", "Designation", designationId);

                // Preserve current filter values
                ViewBag.FilterName = name;
                ViewBag.FilterActive = active;
                ViewBag.FilterStudyLeave = studyLeave;

                return View(faculties);
            }
            catch (Exception)
            {
                ViewBag.FilterDepartments = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewBag.FilterDesignations = new SelectList(Enumerable.Empty<SelectListItem>());
                return View(new List<tblFaculty>());
            }
        }

        // GET: FacultyInformation/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title");
            ViewBag.DesignationID = new SelectList(db.tblDesignations.OrderBy(d => d.Designation), "DesignationID", "Designation");

            var model = new tblFaculty
            {
                Active = true,
                Date = DateTime.Now
            };
            return View(model);
        }

        // POST: FacultyInformation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Date,UserID,DepartmentID,DesignationID,AdDesignation,OrderNo,HRNo,Photo,FullName,Qualification,Gender,ResearchInterests,TelNo,Extension,Email,StudyLeave,Active")] tblFaculty model, System.Web.HttpPostedFileBase photoFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // First save the faculty (without photo) to get the FacultyID
                    db.tblFaculties.Add(model);
                    db.SaveChanges();

                    // Process photo after ID is available
                    if (photoFile != null && photoFile.ContentLength > 0)
                    {
                        var ext = Path.GetExtension(photoFile.FileName)?.ToLowerInvariant();
                        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        if (!allowed.Contains(ext))
                        {
                            ModelState.AddModelError("Photo", "Invalid image type. Allowed: jpg, jpeg, png, gif, webp.");
                            goto ReturnView;
                        }
                        if (photoFile.ContentLength > (2 * 1024 * 1024))
                        {
                            ModelState.AddModelError("Photo", "Image is too large. Max 2MB.");
                            goto ReturnView;
                        }
                        var physicalDir = Server.MapPath(FacultyImageVirtualDir);
                        if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);
                        var fileName = $"faculty{model.FacultyID}{ext}"; // naming convention
                        var fullPath = Path.Combine(physicalDir, fileName);
                        photoFile.SaveAs(fullPath);
                        model.Photo = fileName;
                        db.Entry(model).State = EntityState.Modified; // update photo field
                        db.SaveChanges();
                    }

                    TempData["Success"] = "Faculty member created successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating faculty member: " + ex.Message);
                }
            }
ReturnView:
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            ViewBag.DesignationID = new SelectList(db.tblDesignations.OrderBy(d => d.Designation), "DesignationID", "Designation", model.DesignationID);
            return View(model);
        }

        // GET: FacultyInformation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var faculty = db.tblFaculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", faculty.DepartmentID);
            ViewBag.DesignationID = new SelectList(db.tblDesignations.OrderBy(d => d.Designation), "DesignationID", "Designation", faculty.DesignationID);

            return View(faculty);
        }

        // POST: FacultyInformation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FacultyID,Date,UserID,DepartmentID,DesignationID,AdDesignation,OrderNo,HRNo,Photo,FullName,Qualification,Gender,ResearchInterests,TelNo,Extension,Email,StudyLeave,Active")] tblFaculty model, System.Web.HttpPostedFileBase photoFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existing = db.tblFaculties.AsNoTracking().FirstOrDefault(f => f.FacultyID == model.FacultyID);
                    if (existing == null) return HttpNotFound();

                    if (photoFile != null && photoFile.ContentLength > 0)
                    {
                        var ext = Path.GetExtension(photoFile.FileName)?.ToLowerInvariant();
                        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        if (!allowed.Contains(ext)) { ModelState.AddModelError("Photo", "Invalid image type. Allowed: jpg, jpeg, png, gif, webp."); goto ReturnEdit; }
                        if (photoFile.ContentLength > (2 * 1024 * 1024)) { ModelState.AddModelError("Photo", "Image is too large. Max 2MB."); goto ReturnEdit; }
                        var physicalDir = Server.MapPath(FacultyImageVirtualDir);
                        if (!Directory.Exists(physicalDir)) Directory.CreateDirectory(physicalDir);
                        var newFileName = $"faculty{model.FacultyID}{ext}"; // naming convention
                        var newFullPath = Path.Combine(physicalDir, newFileName);

                        // Delete old file if different name
                        if (!string.IsNullOrEmpty(existing.Photo))
                        {
                            var oldPath = Path.Combine(physicalDir, existing.Photo);
                            if (System.IO.File.Exists(oldPath)) { try { System.IO.File.Delete(oldPath); } catch { } }
                        }
                        photoFile.SaveAs(newFullPath);
                        model.Photo = newFileName;
                    }
                    else
                    {
                        model.Photo = existing.Photo; // keep current
                    }

                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Success"] = "Faculty member updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating faculty member: " + ex.Message);
                }
            }
ReturnEdit:
            ViewBag.DepartmentID = new SelectList(db.tblDepartments.OrderBy(d => d.Title), "DepartmentID", "Title", model.DepartmentID);
            ViewBag.DesignationID = new SelectList(db.tblDesignations.OrderBy(d => d.Designation), "DesignationID", "Designation", model.DesignationID);
            return View(model);
        }

        // GET: FacultyInformation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var faculty = db.tblFaculties.Find(id);
            if (faculty == null)
            {
                return HttpNotFound();
            }

            return View(faculty);
        }

        // POST: FacultyInformation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var faculty = db.tblFaculties.Find(id);
                if (faculty == null)
                {
                    if (Request.IsAjaxRequest()) return Json(new { success = false, message = "Faculty member not found" });
                    return RedirectToAction("Index");
                }

                // Optional: remove photo file when deleting
                if (!string.IsNullOrEmpty(faculty.Photo))
                {
                    var physicalDir = Server.MapPath(FacultyImageVirtualDir);
                    var photoPath = Path.Combine(physicalDir, faculty.Photo);
                    if (System.IO.File.Exists(photoPath)) { try { System.IO.File.Delete(photoPath); } catch { } }
                }

                db.tblFaculties.Remove(faculty);
                db.SaveChanges();

                if (Request.IsAjaxRequest()) return Json(new { success = true, message = "Faculty member deleted successfully" });
                TempData["Success"] = "Faculty member deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest()) return Json(new { success = false, message = "Error deleting faculty member: " + ex.Message });
                ModelState.AddModelError("", "Error deleting faculty member: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
