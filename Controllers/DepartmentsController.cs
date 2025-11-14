using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using HitecCM.Models;
using System.IO;
using System.Web;

namespace HitecCM.Controllers
{
    public class DepartmentsController : BaseController
    {
        private Model1 db = new Model1();

        // GET: Departments
        public ActionResult Index()
        {
            try
            {
                var departments = db.tblDepartments
                    .OrderBy(d => d.C_Order)
                    .ThenBy(d => d.Title)
                    .ToList();
                return View(departments);
            }
            catch (Exception ex)
            {
                return View(new System.Collections.Generic.List<tblDepartment>());
            }
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View(new tblDepartment());
        }

        // POST: Departments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Code,Title,Banner,About,HODPhoto,HODMessage,Programs,Contacts,C_Order")] tblDepartment department, HttpPostedFileBase banner_file, HttpPostedFileBase hod_photo_file)
        {
            if (ModelState.IsValid)
            {
                // Handle banner image upload
                if (banner_file != null && banner_file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(banner_file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img/departments/"), fileName);
                    banner_file.SaveAs(path);
                    department.Banner = fileName;
                }

                // Handle HOD photo upload
                if (hod_photo_file != null && hod_photo_file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(hod_photo_file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img/departments/"), fileName);
                    hod_photo_file.SaveAs(path);
                    department.HODPhoto = fileName;
                }

                db.tblDepartments.Add(department);
                db.SaveChanges();
                TempData["Success"] = "Department created successfully.";
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDepartment department = db.tblDepartments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "DepartmentID,Code,Title,Banner,About,HODPhoto,HODMessage,Programs,Contacts,C_Order")] tblDepartment department, HttpPostedFileBase banner_file, HttpPostedFileBase hod_photo_file)
        {
            if (ModelState.IsValid)
            {
                var existingDepartment = db.tblDepartments.Find(department.DepartmentID);
                if (existingDepartment != null)
                {
                    // Handle banner image upload
                    if (banner_file != null && banner_file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(banner_file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/img/departments/"), fileName);
                        banner_file.SaveAs(path);
                        department.Banner = fileName;
                    }
                    else
                    {
                        // Keep existing banner if no new file uploaded
                        department.Banner = existingDepartment.Banner;
                    }

                    // Handle HOD photo upload
                    if (hod_photo_file != null && hod_photo_file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(hod_photo_file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/img/departments/"), fileName);
                        hod_photo_file.SaveAs(path);
                        department.HODPhoto = fileName;
                    }
                    else
                    {
                        // Keep existing HOD photo if no new file uploaded
                        department.HODPhoto = existingDepartment.HODPhoto;
                    }
                }

                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Department updated successfully.";
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblDepartment department = db.tblDepartments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblDepartment department = db.tblDepartments.Find(id);
            if (department != null)
            {
                db.tblDepartments.Remove(department);
                db.SaveChanges();
                TempData["Success"] = "Department deleted successfully.";
            }
            return RedirectToAction("Index");
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